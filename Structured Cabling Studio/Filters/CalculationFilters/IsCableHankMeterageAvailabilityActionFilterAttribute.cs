﻿using Microsoft.AspNetCore.Mvc.Filters;

namespace StructuredCablingStudio.Filters.CalculationFilters
{
	public class IsCableHankMeterageAvailabilityActionFilterAttribute : ActionFilterAttribute
	{
		public override void OnActionExecuted(ActionExecutedContext context)
		{
			object? rawValue = context.ModelState.GetValueOrDefault("IsCableHankMeterageAvailability")?.RawValue;
			if (rawValue is not null)
			{
				if (rawValue.GetType().IsArray)
				{
					var array = (Array)rawValue;
					var stringValues = new string[array.Length];
					if (stringValues[0] == "false")
					{
						context.ModelState.SetModelValue("CableHankMeterage", "", default);
					}
				}
				if (rawValue is string)
				{
					string stringValue = (string)rawValue;
					if (stringValue == "false")
					{
						context.ModelState.SetModelValue("CableHankMeterage", "", default);
					}
				}
			}
		}
	}
}
