using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace Jarvis.SstCloud.Core.Model
{
	public class HouseInfo : IdentifiableValue
	{
		/*
		{
		    "id": 9086,
		    "owner": "just.skorik@yandex.ru",
		    "workdays": {
		      "id": 9080,
		      "current_day": "vacation",
		      "workdays_count": 2,
		      "vacations_count": 2,
		      "current_week": 0,
		      "is_custom": false,
		      "vacations": [
		        5,
		        6
		      ],
		      "start_date": null,
		      "next_workday": null,
		      "next_vacation": null,
		      "start_day": 0,
		      "house": 9086
		    },
		    "timezone": "Europe/Moscow",
		    "created_at": "2019-03-14T15:36:30.228341Z",
		    "updated_at": "2019-04-13T14:32:55.228255Z",
		    "uid": "3f94a81536",
		    "name": "67",
		    "in_home": true,
		    "behaviour": "noth",
		    "close_valves": 2,
		    "report_date": 24,
		    "users": [
		      5388
		    ]
		  }
		*/
		public class WorkDaysInfo : IdentifiableValue
		{
			[JsonProperty("current_day")]
			public DayType CurrentDayType { get; set; }

			[JsonProperty("workdays_count")]
			public int WorkDaysCount { get; set; }

			public int VacationsCount => VacationDaysIndexes?.Count ?? 0;

			/// <summary>
			/// Gets or sets the vacation days indexes. For sasurday and sunday returns [5,6]
			/// </summary>
			/// <value>
			/// The vacation days indexes.
			/// </value>
			[JsonProperty("vacations")]
			public List<int> VacationDaysIndexes { set; get; }

			[JsonProperty("house")]
			public int HouseOwnerId { set; get; }
		}

		[JsonProperty("owner")]
		public string Owner { get; set; }

		[JsonProperty("workdays")]
		public WorkDaysInfo WorkDays { get; set; }

		[JsonProperty("timezone")]
		public string TimeZoneName { get; set; }

		[JsonProperty("created_at")]
		public DateTime CreatedAt { get; set; }

		[JsonProperty("updated_at")]
		public DateTime UpdatedAt { get; set; }

		[JsonProperty("uid")]
		public string UniqueId { get; set; }

		[JsonProperty("name")]
		public string Name { get; set; }

		[JsonProperty("in_home")]
		public bool IsAtHome { get; set; }

		[JsonProperty("close_valves")]
		public int CloseValves { get; set; }

		[JsonProperty("report_date")]
		public int WaterCountersReportDay { get; set; }

		[JsonProperty("users")]
		public List<int> HouseUserIds { get; set; }
	}
}
