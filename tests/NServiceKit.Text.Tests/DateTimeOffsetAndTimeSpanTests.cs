﻿using System;
using NUnit.Framework;
#if !MONOTOUCH
using NServiceKit.ServiceModel.Serialization;
#endif

namespace NServiceKit.Text.Tests
{
    /// <summary>A date time offset and time span tests.</summary>
    [TestFixture]
    public class DateTimeOffsetAndTimeSpanTests : TestBase
    {
#if !MONOTOUCH
        /// <summary>Tests fixture set up.</summary>
        [TestFixtureSetUp]
        public void TestFixtureSetUp()
        {
            JsonDataContractSerializer.Instance.UseBcl = true;
        }

        /// <summary>Tests fixture tear down.</summary>
        [TestFixtureTearDown]
        public void TestFixtureTearDown()
        {
            JsonDataContractSerializer.Instance.UseBcl = false;            
        }
#endif

        /// <summary>Can serializable date time offset field.</summary>
        [Test]
        public void Can_Serializable_DateTimeOffset_Field()
        {
            var model = new SampleModel { Id = 1, Date = new DateTimeOffset(2012, 6, 27, 11, 26, 04, 524, TimeSpan.FromHours(7)) };

            //Behaviour of .NET's BCL classes
            //JsonDataContractSerializer.Instance.SerializeToString(model).Print();
            //DataContractSerializer.Instance.Parse(model).Print();

            var json = JsonSerializer.SerializeToString(model);
            Assert.That(json, Is.StringContaining("\"TimeSpan\":\"PT0S\""));

            var fromJson = json.FromJson<SampleModel>();

            Assert.That(model.Date, Is.EqualTo(fromJson.Date));
            Assert.That(model.TimeSpan, Is.EqualTo(fromJson.TimeSpan));

            Serialize(fromJson);
        }

        /// <summary>Can serialize time span field.</summary>
        [Test]
        public void Can_serialize_TimeSpan_field()
        {
            var fromDate = new DateTime(2069, 01, 02);
            var toDate = new DateTime(2079, 01, 02);
            var period = toDate - fromDate;

            var model = new SampleModel { Id = 1, TimeSpan = period };
            var json = JsonSerializer.SerializeToString(model);
            Assert.That(json, Is.StringContaining("\"TimeSpan\":\"P3652D\""));

            //Behaviour of .NET's BCL classes
            //JsonDataContractSerializer.Instance.SerializeToString(model).Print();
            //DataContractSerializer.Instance.Parse(model).Print();

            Serialize(model);
        }

        /// <summary>Can serialize time span field with standard time span format.</summary>
        [Test]
        public void Can_serialize_TimeSpan_field_with_StandardTimeSpanFormat()
        {
            using (JsConfig.With(timeSpanHandler:JsonTimeSpanHandler.StandardFormat))
            {
                var period = TimeSpan.FromSeconds(70);

                var model = new SampleModel { Id = 1, TimeSpan = period };
                var json = JsonSerializer.SerializeToString(model);
                Assert.That(json, Is.StringContaining("\"TimeSpan\":\"00:01:10\""));
            }
        }

        /// <summary>Can serialize nullable time span field with standard time span format.</summary>
        [Test]
        public void Can_serialize_NullableTimeSpan_field_with_StandardTimeSpanFormat()
        {
            using (JsConfig.With(timeSpanHandler: JsonTimeSpanHandler.StandardFormat))
            {
                var period = TimeSpan.FromSeconds(70);

                var model = new NullableSampleModel { Id = 1, TimeSpan = period };
                var json = JsonSerializer.SerializeToString(model);
                Assert.That(json, Is.StringContaining("\"TimeSpan\":\"00:01:10\""));
            }
        }

        /// <summary>Can serialize null time span field with standard time span format.</summary>
        [Test]
        public void Can_serialize_NullTimeSpan_field_with_StandardTimeSpanFormat()
        {
            using (JsConfig.With(timeSpanHandler: JsonTimeSpanHandler.StandardFormat))
            {
                var model = new NullableSampleModel { Id = 1 };
                var json = JsonSerializer.SerializeToString(model);
                Assert.That(json, Is.Not.StringContaining("\"TimeSpan\""));
            }
        }

        /// <summary>A data Model for the sample.</summary>
        public class SampleModel
        {
            /// <summary>Gets or sets the identifier.</summary>
            /// <value>The identifier.</value>
            public int Id { get; set; }

            /// <summary>Gets or sets the date.</summary>
            /// <value>The date.</value>
            public DateTimeOffset Date { get; set; }

            /// <summary>Gets or sets the time span.</summary>
            /// <value>The time span.</value>
            public TimeSpan TimeSpan { get; set; }
        }

        /// <summary>A data Model for the nullable sample.</summary>
        public class NullableSampleModel
        {
            /// <summary>Gets or sets the identifier.</summary>
            /// <value>The identifier.</value>
            public int Id { get; set; }

            /// <summary>Gets or sets the date.</summary>
            /// <value>The date.</value>
            public DateTimeOffset Date { get; set; }

            /// <summary>Gets or sets the time span.</summary>
            /// <value>The time span.</value>
            public TimeSpan? TimeSpan { get; set; }
        }
    }
}
