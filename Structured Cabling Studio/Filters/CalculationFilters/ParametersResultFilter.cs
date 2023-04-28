﻿using AutoMapper;
using Microsoft.AspNetCore.Mvc.Filters;
using StructuredCablingStudio.Controllers;
using StructuredCablingStudio.DTOs;
using StructuredCablingStudio.Extensions.ISessionExtension;
using StructuredCablingStudio.Models.ViewModels.CalculationViewModels;
using StructuredCablingStudioCore.Calculation;
using StructuredCablingStudioCore.Parameters;

namespace StructuredCablingStudio.Filters.CalculationFilters
{
	public class ParametersResultFilter : IResultFilter
	{
		private readonly IMapper _mapper;

		public ParametersResultFilter(IMapper mapper)
		{
			_mapper = mapper;
		}

		public void OnResultExecuted(ResultExecutedContext context)
		{
		}

		public void OnResultExecuting(ResultExecutingContext context)
		{
			var controller = (Calculation)context.Controller;
			var model = (CalculateViewModel?)controller.ViewData.Model;
			if(model != null)
			{
				var structuredCablingStudioParameters = _mapper.Map<StructuredCablingStudioParameters>(model);
				controller.ViewData["Diapasons"] = structuredCablingStudioParameters.Diapasons;
				var structuredCablingParameters = _mapper.Map<StructuredCablingParameters>(structuredCablingStudioParameters);
				context.HttpContext.Session.SetStructuredCablingParameters(structuredCablingParameters);
				var configurationCalulateParameters = _mapper.Map<ConfigurationCalculateParameters>(model);
				var calculateParameters = _mapper.Map<CalculateParameters>(configurationCalulateParameters);
				context.HttpContext.Session.SetCalculateParameters(calculateParameters);
			}
		}
	}
}