using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;

namespace Jarvis.SstCloud.Client.Model
{
	public class WaterCounterInfo : IdentifiableValue
	{
		/*
		[
		  {
		    "id": 39871,
		    "name": "Cold",
		    "line": 4,
		    "for_hot_water": false,
		    "max_value": 0,
		    "step": 10,
		    "value": 319881,
		    "prev_value": 319881,
		    "prev_step": 10,
		    "device": 29485
		  },
		  {
		    "id": 39870,
		    "name": "Hot",
		    "line": 3,
		    "for_hot_water": true,
		    "max_value": 0,
		    "step": 10,
		    "value": 346911,
		    "prev_value": 346911,
		    "prev_step": 10,
		    "device": 29485
		  }
		]
		*/
		[JsonProperty("name")]
		public string Name { get; set; }

		[JsonProperty("line")]
		public int Line { get; set; }

		[JsonProperty("for_hot_water")]
		public bool IsHotWaterCounter { get; set; }

		[JsonProperty("max_value")]
		public int MaxValue { get; set; }

		[JsonProperty("step")]
		public int CubicMetersPerImpulse { get; set; }

		[JsonProperty("value")]
		public int Value { get; set; }
	}
}
