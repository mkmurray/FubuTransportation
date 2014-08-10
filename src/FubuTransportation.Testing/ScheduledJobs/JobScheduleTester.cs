﻿using System;
using System.Diagnostics;
using System.Linq;
using FubuCore;
using FubuTestingSupport;
using FubuTransportation.ScheduledJobs;
using NUnit.Framework;

namespace FubuTransportation.Testing.ScheduledJobs
{
    [TestFixture]
    public class JobScheduleTester
    {
        [Test]
        public void tracking_schedule_changes()
        {
            var schedule = new JobSchedule(new[]
            {
                new JobStatus(typeof (AJob), DateTime.Today),
                new JobStatus(typeof (BJob), DateTime.Today),
            });

            schedule.Schedule(typeof (AJob), DateTime.Today.AddHours(5));
            schedule.Schedule(typeof (CJob), DateTime.Today.AddHours(5));
        
            schedule.Changes().Select(x => x.JobType)
                .ShouldHaveTheSameElementsAs(typeof(AJob), typeof(CJob));
        }

        [Test]
        public void remove_obsolete_jobs()
        {
            var schedule = new JobSchedule(new[]
            {
                new JobStatus(typeof (AJob), DateTime.Today),
                new JobStatus(typeof (BJob), DateTime.Today),
            });

            schedule.RemoveObsoleteJobs(new Type[]{typeof(AJob), typeof(CJob)});

            schedule.Removals().Single()
                .JobType.ShouldEqual(typeof (BJob));
        }

        [Test]
        public void removing_an_obsolete_removes_it_from_the_active_list()
        {
            var schedule = new JobSchedule(new[]
            {
                new JobStatus(typeof (AJob), DateTime.Today),
                new JobStatus(typeof (BJob), DateTime.Today),
            });

            schedule.RemoveObsoleteJobs(new Type[] { typeof(AJob), typeof(CJob) });

            schedule.Any(x => x.JobType == typeof(BJob)).ShouldBeFalse();
        }

        [Test]
        public void rescheduling_a_job_tracks_it_as_a_change()
        {
            var schedule = new JobSchedule(new[]
            {
                new JobStatus(typeof (AJob), DateTime.Today.AddHours(-1)),
                new JobStatus(typeof (BJob), DateTime.Today.AddHours(2)),
                new JobStatus(typeof (CJob), DateTime.Today.AddMinutes(1)),
            });

            schedule.Schedule(typeof (AJob), DateTime.Today);
            schedule.Schedule(typeof (CJob), DateTime.Today);

            schedule.Changes().Select(x => x.JobType)
                .ShouldHaveTheSameElementsAs(typeof (AJob), typeof (CJob));
        }
    }
}