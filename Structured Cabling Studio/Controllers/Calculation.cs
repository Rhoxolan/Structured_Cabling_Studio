﻿using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StructuredCablingStudio.Data.Contexts;
using StructuredCablingStudio.Data.Entities;
using StructuredCablingStudio.Extensions.ISessionExtension;
using StructuredCablingStudio.Models.ViewModels.CalculationViewModels;
using StructuredCablingStudioCore;
using StructuredCablingStudioCore.Calculation;
using StructuredCablingStudioCore.Parameters;
using System.Security.Claims;
using System.Text.Json.Serialization;
using System.Text.Json;
using System.Text;
using Microsoft.Extensions.Localization;
using static System.Convert;
using static System.DateTimeOffset;
using static System.Text.Encoding;
using static System.String;
using StructuredCablingStudio.DTOs.CalculateDTOs;
using StructuredCablingStudio.Filters.CalculationFilters;

namespace StructuredCablingStudio.Controllers
{
	public class Calculation : Controller
	{
		private readonly ILogger<Calculation> _logger;
		private readonly ApplicationContext _context;
		private readonly UserManager<User> _userManager;
		private readonly SignInManager<User> _signInManager;
		private readonly RoleManager<IdentityRole> _roleManager;
		private readonly IMapper _mapper;
		private readonly IStringLocalizer<Calculation> _localizer;

		public Calculation(ILogger<Calculation> logger, ApplicationContext context, UserManager<User> userManager,
			SignInManager<User> signInManager, RoleManager<IdentityRole> roleManager, IMapper mapper, IStringLocalizer<Calculation> localizer)
		{
			_logger = logger;
			_context = context;
			_userManager = userManager;
			_signInManager = signInManager;
			_roleManager = roleManager;
			_mapper = mapper;
			_localizer = localizer;
		}

		public IActionResult Calculate()
		{
			return View();
		}

		/// <summary>
		/// Returns the partial view with the clean Calculate form
		/// </summary>
		[HttpPut]
		public IActionResult LoadCalculateForm(StructuredCablingStudioParameters cablingParameters, ConfigurationCalculateParameters calculateParameters,
			CalculateDTO calculateDTO)
		{
			CalculateViewModel viewModel = _mapper.Map<CalculateViewModel>(cablingParameters);
			viewModel.IsCableHankMeterageAvailability = calculateParameters.IsCableHankMeterageAvailability.GetValueOrDefault();
			viewModel.CableHankMeterage = calculateParameters.CableHankMeterage;
			viewModel.MinPermanentLink = calculateDTO.MinPermanentLink;
			viewModel.MaxPermanentLink = calculateDTO.MaxPermanentLink;
			viewModel.NumberOfPorts = calculateDTO.NumberOfPorts;
			viewModel.NumberOfWorkplaces = calculateDTO.NumberOfWorkplaces;
			ViewData["Diapasons"] = cablingParameters.Diapasons;
			return PartialView("_CalculateFormPartial", viewModel);
		}

		/// <summary>
		/// Returns the partial view with the clean display of structured cabling configuration
		/// </summary>
		[HttpPut]
		public IActionResult LoadConfigurationDisplay()
		{
			return PartialView("_ConfigurationDisplayPartial");
		}

		/// <summary>
		/// Edits the viewmodel data from the Calculate form after applying the "StrictComplianceWithTheStandart" setting
		/// </summary>
		/// <param name="calculateVM">The viewmodel form values</param>
		/// <returns>The partial view with the Calculate form</returns>
		[HttpPut]
		[ServiceFilter(typeof(DiapasonActionFilter))]
		[ServiceFilter(typeof(StructuredCablingStudioParametersResultFilter))]
		[ServiceFilter(typeof(ConfigurationCalulateParametersResultFilter))]
		[ServiceFilter(typeof(CalculateDTOResultFilter))]
		public IActionResult PutStrictComplianceWithTheStandart(CalculateViewModel calculateVM)
		{
			if (!calculateVM.IsStrictComplianceWithTheStandart)
			{
				calculateVM.IsAnArbitraryNumberOfPorts = true;
				ModelState.SetModelValue(nameof(calculateVM.IsAnArbitraryNumberOfPorts), calculateVM.IsAnArbitraryNumberOfPorts, default);
			}
			return PartialView("_CalculateFormPartial", calculateVM);
		}

		/// <summary>
		/// Edits the viewmodel data from the Calculate form after applying the "RecommendationsAvailability" setting
		/// </summary>
		/// <param name="calculateVM">The viewmodel form values</param>
		/// <returns>The partial view with the Calculate form</returns>
		[HttpPut]
		[ServiceFilter(typeof(DiapasonActionFilter))]
		[ServiceFilter(typeof(StructuredCablingStudioParametersResultFilter))]
		[ServiceFilter(typeof(ConfigurationCalulateParametersResultFilter))]
		[ServiceFilter(typeof(CalculateDTOResultFilter))]
		public IActionResult PutRecommendationsAvailability(CalculateViewModel calculateVM)
		{
			if (!calculateVM.IsRecommendationsAvailability)
			{
				calculateVM.IsCableRouteRunOutdoors = false;
				calculateVM.IsConsiderFireSafetyRequirements = false;
				calculateVM.IsCableShieldingNecessity = false;
				calculateVM.HasTenBase_T = false;
				calculateVM.HasFastEthernet = false;
				calculateVM.HasGigabitBASE_T = false;
				calculateVM.HasGigabitBASE_TX = false;
				calculateVM.HasTwoPointFiveGBASE_T = false;
				calculateVM.HasFiveGBASE_T = false;
				calculateVM.HasTenGE = false;
				ModelState.SetModelValue(nameof(calculateVM.IsCableRouteRunOutdoors), calculateVM.IsCableRouteRunOutdoors, default);
				ModelState.SetModelValue(nameof(calculateVM.IsConsiderFireSafetyRequirements), calculateVM.IsConsiderFireSafetyRequirements, default);
				ModelState.SetModelValue(nameof(calculateVM.IsCableShieldingNecessity), calculateVM.IsCableShieldingNecessity, default);
				ModelState.SetModelValue(nameof(calculateVM.HasTenBase_T), calculateVM.HasTenBase_T, default);
				ModelState.SetModelValue(nameof(calculateVM.HasFastEthernet), calculateVM.HasFastEthernet, default);
				ModelState.SetModelValue(nameof(calculateVM.HasGigabitBASE_T), calculateVM.HasGigabitBASE_T, default);
				ModelState.SetModelValue(nameof(calculateVM.HasGigabitBASE_TX), calculateVM.HasGigabitBASE_TX, default);
				ModelState.SetModelValue(nameof(calculateVM.HasTwoPointFiveGBASE_T), calculateVM.HasTwoPointFiveGBASE_T, default);
				ModelState.SetModelValue(nameof(calculateVM.HasFiveGBASE_T), calculateVM.HasFiveGBASE_T, default);
				ModelState.SetModelValue(nameof(calculateVM.HasTenGE), calculateVM.HasTenGE, default);
			}
			return PartialView("_CalculateFormPartial", calculateVM);
		}

		/// <summary>
		/// Edits the viewmodel data from the Сalculate form after applying the "CableHankMeterageAvailability" setting
		/// </summary>
		/// <param name="calculateVM">The viewmodel form values</param>
		/// <returns>The partial view with the Calculate form</returns>
		[HttpPut]
		[ServiceFilter(typeof(DiapasonActionFilter))]
		[ServiceFilter(typeof(StructuredCablingStudioParametersResultFilter))]
		[ServiceFilter(typeof(ConfigurationCalulateParametersResultFilter))]
		[ServiceFilter(typeof(CalculateDTOResultFilter))]
		public IActionResult PutCableHankMeterageAvailability(CalculateViewModel calculateVM)
		{
			var configurationCalculateParameters = _mapper.Map<ConfigurationCalculateParameters>(calculateVM);
			if (calculateVM.CableHankMeterage != configurationCalculateParameters.CableHankMeterage)
			{
				calculateVM.CableHankMeterage = configurationCalculateParameters.CableHankMeterage;
				ModelState.SetModelValue(nameof(calculateVM.CableHankMeterage), calculateVM.CableHankMeterage, default);
			}
			var ceiledAveragePermanentLink =
				(int)Math.Ceiling((calculateVM.MinPermanentLink + calculateVM.MaxPermanentLink) / 2 * calculateVM.TechnologicalReserve);
			if (calculateVM.CableHankMeterage < ceiledAveragePermanentLink)
			{
				calculateVM.CableHankMeterage = ceiledAveragePermanentLink;
				ModelState.SetModelValue(nameof(calculateVM.CableHankMeterage), calculateVM.CableHankMeterage, default);
			}
			return PartialView("_CalculateFormPartial", calculateVM);
		}

		/// <summary>
		/// Edits the viewmodel data from the Сalculate form after applying the "AnArbitraryNumberOfPorts" setting
		/// </summary>
		/// <param name="calculateVM">The viewmodel form values</param>
		/// <returns>The partial view with the Calculate form</returns>
		[HttpPut]
		[ServiceFilter(typeof(DiapasonActionFilter))]
		[ServiceFilter(typeof(StructuredCablingStudioParametersResultFilter))]
		[ServiceFilter(typeof(ConfigurationCalulateParametersResultFilter))]
		[ServiceFilter(typeof(CalculateDTOResultFilter))]
		public IActionResult PutAnArbitraryNumberOfPorts(CalculateViewModel calculateVM)
		{
			return PartialView("_CalculateFormPartial", calculateVM);
		}

		/// <summary>
		/// Edits the viewmodel data from the Сalculate form after applying the "TechnologicalReserveAvailability" setting
		/// </summary>
		/// <param name="calculateVM">The viewmodel form values</param>
		/// <returns>The partial view with the Calculate form</returns>
		[HttpPut]
		[ServiceFilter(typeof(DiapasonActionFilter))]
		[ServiceFilter(typeof(StructuredCablingStudioParametersResultFilter))]
		[ServiceFilter(typeof(ConfigurationCalulateParametersResultFilter))]
		[ServiceFilter(typeof(CalculateDTOResultFilter))]
		public IActionResult PutTechnologicalReserveAvailability(CalculateViewModel calculateVM)
		{
			var structuredCablingStudioParameters = _mapper.Map<StructuredCablingStudioParameters>(calculateVM);
			if (calculateVM.TechnologicalReserve != structuredCablingStudioParameters.TechnologicalReserve)
			{
				calculateVM.TechnologicalReserve = structuredCablingStudioParameters.TechnologicalReserve;
				ModelState.SetModelValue(nameof(calculateVM.TechnologicalReserve), calculateVM.TechnologicalReserve, default);
			}
			return PartialView("_CalculateFormPartial", calculateVM);
		}

		/// <summary>
		/// Restore defaults settings on viewmodel data to the Сalculate form
		/// </summary>
		/// <param name="calculateVM">The viewmodel form values</param>
		/// <returns>The partial view with the Calculate form</returns>
		[HttpPut]
		[ServiceFilter(typeof(StructuredCablingStudioParametersResultFilter))]
		[ServiceFilter(typeof(ConfigurationCalulateParametersResultFilter))]
		[ServiceFilter(typeof(CalculateDTOResultFilter))]
		public IActionResult RestoreDefaultsCalculateForm(CalculateViewModel calculateVM)
		{
			var cablingParameters = new StructuredCablingStudioParameters
			{
				IsStrictComplianceWithTheStandart = true,
				IsAnArbitraryNumberOfPorts = true,
				IsTechnologicalReserveAvailability = true,
				IsRecommendationsAvailability = true
			};
			cablingParameters.RecommendationsArguments.IsolationType = IsolationType.Indoor;
			cablingParameters.RecommendationsArguments.IsolationMaterial = IsolationMaterial.LSZH;
			cablingParameters.RecommendationsArguments.ShieldedType = ShieldedType.UTP;
			cablingParameters.RecommendationsArguments.ConnectionInterfaces = new List<ConnectionInterfaceStandard>
			{
				ConnectionInterfaceStandard.FastEthernet,
				ConnectionInterfaceStandard.GigabitBASE_T
			};
			var calculateParameters = new ConfigurationCalculateParameters
			{
				IsCableHankMeterageAvailability = true
			};
			calculateVM.IsCableHankMeterageAvailability = calculateParameters.IsCableHankMeterageAvailability.Value;
			calculateVM.CableHankMeterage = calculateParameters.CableHankMeterage;
			calculateVM.TechnologicalReserve = cablingParameters.TechnologicalReserve;
			calculateVM.IsStrictComplianceWithTheStandart = cablingParameters.IsStrictComplianceWithTheStandart.Value;
			calculateVM.IsAnArbitraryNumberOfPorts = cablingParameters.IsAnArbitraryNumberOfPorts.Value;
			calculateVM.IsTechnologicalReserveAvailability = cablingParameters.IsTechnologicalReserveAvailability.Value;
			calculateVM.IsRecommendationsAvailability = cablingParameters.IsRecommendationsAvailability.Value;
			calculateVM.IsCableRouteRunOutdoors = cablingParameters.RecommendationsArguments.IsolationType == IsolationType.Outdoor;
			calculateVM.IsConsiderFireSafetyRequirements = cablingParameters.RecommendationsArguments.IsolationMaterial == IsolationMaterial.LSZH;
			calculateVM.IsCableShieldingNecessity = cablingParameters.RecommendationsArguments.ShieldedType == ShieldedType.FTP;
			calculateVM.HasTenBase_T
				= cablingParameters.RecommendationsArguments.ConnectionInterfaces.Contains(ConnectionInterfaceStandard.TenBASE_T);
			calculateVM.HasFastEthernet
				= cablingParameters.RecommendationsArguments.ConnectionInterfaces.Contains(ConnectionInterfaceStandard.FastEthernet);
			calculateVM.HasGigabitBASE_T
				= cablingParameters.RecommendationsArguments.ConnectionInterfaces.Contains(ConnectionInterfaceStandard.GigabitBASE_T);
			calculateVM.HasGigabitBASE_TX
				= cablingParameters.RecommendationsArguments.ConnectionInterfaces.Contains(ConnectionInterfaceStandard.GigabitBASE_TX);
			calculateVM.HasTwoPointFiveGBASE_T
				= cablingParameters.RecommendationsArguments.ConnectionInterfaces.Contains(ConnectionInterfaceStandard.TwoPointFiveGBASE_T);
			calculateVM.HasFiveGBASE_T
				= cablingParameters.RecommendationsArguments.ConnectionInterfaces.Contains(ConnectionInterfaceStandard.FiveGBASE_T);
			calculateVM.HasTenGE
				= cablingParameters.RecommendationsArguments.ConnectionInterfaces.Contains(ConnectionInterfaceStandard.TenGE);
			ModelState.SetModelValue(nameof(calculateVM.IsCableHankMeterageAvailability), calculateVM.IsCableHankMeterageAvailability, default);
			ModelState.SetModelValue(nameof(calculateVM.CableHankMeterage), calculateVM.CableHankMeterage, default);
			ModelState.SetModelValue(nameof(calculateVM.TechnologicalReserve), calculateVM.TechnologicalReserve, default);
			ModelState.SetModelValue(nameof(calculateVM.IsStrictComplianceWithTheStandart), calculateVM.IsStrictComplianceWithTheStandart, default);
			ModelState.SetModelValue(nameof(calculateVM.IsAnArbitraryNumberOfPorts), calculateVM.IsAnArbitraryNumberOfPorts, default);
			ModelState.SetModelValue(nameof(calculateVM.IsTechnologicalReserveAvailability), calculateVM.IsTechnologicalReserveAvailability, default);
			ModelState.SetModelValue(nameof(calculateVM.IsRecommendationsAvailability), calculateVM.IsRecommendationsAvailability, default);
			ModelState.SetModelValue(nameof(calculateVM.IsCableRouteRunOutdoors), calculateVM.IsCableRouteRunOutdoors, default);
			ModelState.SetModelValue(nameof(calculateVM.IsConsiderFireSafetyRequirements), calculateVM.IsConsiderFireSafetyRequirements, default);
			ModelState.SetModelValue(nameof(calculateVM.IsCableShieldingNecessity), calculateVM.IsCableShieldingNecessity, default);
			ModelState.SetModelValue(nameof(calculateVM.HasTenBase_T), calculateVM.HasTenBase_T, default);
			ModelState.SetModelValue(nameof(calculateVM.HasFastEthernet), calculateVM.HasFastEthernet, default);
			ModelState.SetModelValue(nameof(calculateVM.HasGigabitBASE_T), calculateVM.HasGigabitBASE_T, default);
			ModelState.SetModelValue(nameof(calculateVM.HasGigabitBASE_TX), calculateVM.HasGigabitBASE_TX, default);
			ModelState.SetModelValue(nameof(calculateVM.HasTwoPointFiveGBASE_T), calculateVM.HasTwoPointFiveGBASE_T, default);
			ModelState.SetModelValue(nameof(calculateVM.HasFiveGBASE_T), calculateVM.HasFiveGBASE_T, default);
			ModelState.SetModelValue(nameof(calculateVM.HasTenGE), calculateVM.HasTenGE, default);
			return PartialView("_CalculateFormPartial", calculateVM);
		}

		/// <summary>
		/// Calculation of the structured cabling configuration
		/// </summary>
		/// <param name="calculateVM">The viewmodel form values</param>
		/// <returns>The partial view with the display of the structured cabling configuration</returns>
		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> Calculate(CalculateViewModel calculateVM)
		{
			var cablingParameters = _mapper.Map<StructuredCablingStudioParameters>(calculateVM);
			var calculateParameters = _mapper.Map<ConfigurationCalculateParameters>(calculateVM);
			var recordTime = FromUnixTimeMilliseconds(ToInt64(calculateVM.RecordTime)).DateTime.ToLocalTime();
			var configuration = calculateParameters.Calculate(cablingParameters, recordTime, calculateVM.MinPermanentLink, calculateVM.MaxPermanentLink,
				calculateVM.NumberOfWorkplaces, calculateVM.NumberOfPorts);
			if (User.Identity != null && User.Identity.IsAuthenticated)
			{
				var userId = User.FindFirst(ClaimTypes.NameIdentifier);
				if (userId != null)
				{
					var currentUser = await _userManager.FindByIdAsync(userId.Value);
					if (currentUser != null)
					{
						var configuratonEntity = _mapper.Map<CablingConfigurationEntity>(configuration);
						configuratonEntity.User = currentUser;
						await _context.CablingConfigurations.AddAsync(configuratonEntity);
						await _context.SaveChangesAsync();
					}
				}
			}
			return PartialView("_ConfigurationDisplayPartial", configuration);
		}


		[Authorize]
		public IActionResult History()
		{
			return View();
		}

		public IActionResult Information()
		{
			return Content("Informatin Page");
		}

		[HttpPost]
		public IActionResult SaveToTXT(string serializedCablingConfiguration)
		{
			var options = new JsonSerializerOptions
			{
				WriteIndented = true,
				ReferenceHandler = ReferenceHandler.Preserve,
			};
			var cablingConfiguration = JsonSerializer.Deserialize<CablingConfiguration>(serializedCablingConfiguration, options);
			if (cablingConfiguration != null)
			{
				var fileName = $"{_localizer["StructuredCablingConfiguration"]} " +
					$"{cablingConfiguration.RecordTime.Day:00}." +
					$"{cablingConfiguration.RecordTime.Month:00}." +
					$"{cablingConfiguration.RecordTime.Year} " +
					$"{cablingConfiguration.RecordTime.Hour:00}." +
					$"{cablingConfiguration.RecordTime.Minute:00}." +
					$"{cablingConfiguration.RecordTime.Second:00}.txt";
				StringBuilder cablingConfigurationSB = new();
				cablingConfigurationSB.AppendLine(_localizer["CreatedIn"]);
				cablingConfigurationSB.AppendLine();
				cablingConfigurationSB.AppendLine();
				cablingConfigurationSB.AppendLine($"{_localizer["RecordTime"]} {cablingConfiguration.RecordTime.ToShortDateString()} " +
					$"{cablingConfiguration.RecordTime.ToLongTimeString()}");
				cablingConfigurationSB.AppendLine($"{_localizer["MinPermanentLink"]} {cablingConfiguration.MinPermanentLink:F2} " +
					$"{_localizer["m"]}");
				cablingConfigurationSB.AppendLine($"{_localizer["MaxPermanentLink"]} {cablingConfiguration.MaxPermanentLink:F2} " +
					$"{_localizer["m"]}");
				cablingConfigurationSB.AppendLine($"{_localizer["AveragePermanentLink"]} {cablingConfiguration.MaxPermanentLink:F2} " +
					$"{_localizer["m"]}");
				cablingConfigurationSB.AppendLine($"{_localizer["NumberOfWorkplaces"]} {cablingConfiguration.NumberOfWorkplaces}");
				cablingConfigurationSB.AppendLine($"{_localizer["NumberOfPorts"]} {cablingConfiguration.NumberOfPorts}");
				if (cablingConfiguration.CableHankMeterage != null)
				{
					cablingConfigurationSB.AppendLine($"{_localizer["CableQuantity"]} {cablingConfiguration.CableQuantity:F2} " +
						$"{_localizer["m"]}");
					cablingConfigurationSB.AppendLine($"{_localizer["CableHankMeterage"]} {cablingConfiguration.CableHankMeterage:F2} " +
						$"{_localizer["m"]}");
					cablingConfigurationSB.AppendLine($"{_localizer["HankQuantity"]} {cablingConfiguration.HankQuantity}");
				}
				cablingConfigurationSB.AppendLine($"{_localizer["TotalCableQuantity"]} {cablingConfiguration.TotalCableQuantity} " +
					$"{_localizer["m"]}");
				if (!IsNullOrEmpty(cablingConfiguration.Recommendations["Insulation Type"]) &&
					!IsNullOrEmpty(cablingConfiguration.Recommendations["Insulation Material"]) &&
					!IsNullOrEmpty(cablingConfiguration.Recommendations["Shielding"]))
				{
					cablingConfigurationSB.AppendLine();
					cablingConfigurationSB.AppendLine(_localizer["CableSelectionRecommendations"]);
					cablingConfigurationSB.AppendLine($"{_localizer["Insulation Type"]} {cablingConfiguration.Recommendations["Insulation Type"]}");
					cablingConfigurationSB.AppendLine($"{_localizer["Insulation Material"]} {cablingConfiguration.Recommendations["Insulation Material"]}");
					if (!IsNullOrEmpty(cablingConfiguration.Recommendations["Standart"]))
					{
						cablingConfigurationSB.AppendLine($"{_localizer["Standart"]} {cablingConfiguration.Recommendations["Standart"]}");
					}
					cablingConfigurationSB.AppendLine($"{_localizer["Shielding"]} {cablingConfiguration.Recommendations["Shielding"]}");
				}
				var stream = new MemoryStream(UTF8.GetBytes(cablingConfigurationSB.ToString()));
				return File(stream, "text/plain", fileName);
			}
			return Empty;
		}
	}
}