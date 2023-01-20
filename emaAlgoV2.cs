#region Using declarations
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Xml.Serialization;
using NinjaTrader.Cbi;
using NinjaTrader.Gui;
using NinjaTrader.Gui.Chart;
using NinjaTrader.Gui.SuperDom;
using NinjaTrader.Gui.Tools;
using NinjaTrader.Data;
using NinjaTrader.NinjaScript;
using NinjaTrader.Core.FloatingPoint;
using NinjaTrader.NinjaScript.Indicators;
using NinjaTrader.NinjaScript.DrawingTools;
#endregion

//This namespace holds Strategies in this folder and is required. Do not change it. 
namespace NinjaTrader.NinjaScript.Strategies
{
	public class OfficialBeta1 : Strategy
	{
		private int Flat;

		private EMA EMA1;
		
		private EMA EMA2;
		private EMA EMA3;
		private EMA EMA4;
		private EMA EMA5;
		private EMA EMA6;
		private EMA EMA7;
		private EMA EMA8;
		private EMA EMA9;

		protected override void OnStateChange()
		{
			if (State == State.SetDefaults)
			{
				Description								= @"Enter the description for your new custom Strategy here.";
				Name									= "OfficialBeta1";
				Calculate								= Calculate.OnBarClose;
				EntriesPerDirection							= 1;
				EntryHandling								= EntryHandling.AllEntries;
				IsExitOnSessionCloseStrategy						= true;
				ExitOnSessionCloseSeconds						= 30;
				IsFillLimitOnTouch							= false;
				MaximumBarsLookBack							= MaximumBarsLookBack.TwoHundredFiftySix;
				OrderFillResolution							= OrderFillResolution.Standard;
				Slippage								= 0;
				StartBehavior								= StartBehavior.WaitUntilFlatSynchronizeAccount;
				TimeInForce								= TimeInForce.Gtc;
				TraceOrders								= false;
				RealtimeErrorHandling							= RealtimeErrorHandling.StopCancelClose;
				StopTargetHandling							= StopTargetHandling.PerEntryExecution;
				BarsRequiredToTrade							= 20;
				// Disable this property for performance gains in Strategy Analyzer optimizations
				// See the Help Guide for additional information
				IsInstantiatedOnEachOptimizationIteration				= true;
				LongSL									= 40;
				LongPT									= 80;
				LongPT2									= 120;
				ShortSL									= 40;
				ShortPT									= 80;
				ShortPT2								=120;
				StartTime								= DateTime.Parse("10:30", System.Globalization.CultureInfo.InvariantCulture);
				StopTime								= DateTime.Parse("14:30", System.Globalization.CultureInfo.InvariantCulture);
				Flat									= 0;
			}
			else if (State == State.Configure)
			{
				AddDataSeries(Data.BarsPeriodType.Minute, 5);
				AddDataSeries(Data.BarsPeriodType.Minute, 8);
				AddDataSeries(Data.BarsPeriodType.Minute, 15);
				AddDataSeries(Data.BarsPeriodType.Minute, 1);
				AddDataSeries(Data.BarsPeriodType.Minute, 2);
			}
			else if (State == State.DataLoaded)
			{				
				EMA1				= EMA(Closes[3], 25);
				EMA2				= EMA(Closes[3], 50);
				EMA3				= EMA(Closes[1], 25);
				EMA4				= EMA(Closes[1], 50);
				EMA5				= EMA(Closes[5], 25);
				EMA6				= EMA(Closes[5], 50);
				EMA7				= EMA(Closes[2], 25);
				EMA8				= EMA(Closes[2], 50);
				EMA9				= EMA(Closes[5], 14);
				
				// Entries
				SetStopLoss(@"Long1", CalculationMode.Ticks, LongSL, false);
				SetStopLoss(@"Long2", CalculationMode.Ticks, LongSL, false);
				SetProfitTarget(@"Long1", CalculationMode.Ticks, LongPT);
				SetProfitTarget(@"Long2", CalculationMode.Ticks, LongPT2);
				SetStopLoss(@"Short1", CalculationMode.Ticks, LongSL, false);
				SetStopLoss(@"Short2", CalculationMode.Ticks, LongSL, false);
				SetProfitTarget(@"Short1", CalculationMode.Ticks, LongPT);
				SetProfitTarget(@"Short2", CalculationMode.Ticks, LongPT2);
			}
		}

		protected override void OnBarUpdate()
		{
			if (BarsInProgress != 0) 
				return;

			if (CurrentBars[0] < 1
			|| CurrentBars[1] < 5
			|| CurrentBars[2] < 5
			|| CurrentBars[3] < 5
			|| CurrentBars[5] < 5)
				return;

			 // Set 1
			if (
				 // Condition group 1
				((CrossAbove(EMA1, EMA2, 5))
				 && (EMA3[5] > EMA4[5])
				 && (EMA5[5] > EMA6[5])
				 && (EMA7[5] > EMA8[5])
				 && (Position.Quantity == Flat)
				 && (Times[0][0].TimeOfDay > StartTime.TimeOfDay)
				 && (Times[0][0].TimeOfDay < StopTime.TimeOfDay))
				 // Condition group 1
				 || ((EMA1[5] > EMA2[5])
				 && (CrossAbove(EMA3, EMA4, 5))
				 && (EMA5[5] > EMA6[5])
				 && (EMA7[5] > EMA8[5])
				 && (Position.Quantity == Flat)
				 && (Times[0][0].TimeOfDay > StartTime.TimeOfDay)
				 && (Times[0][0].TimeOfDay < StopTime.TimeOfDay))
				 // Condition group 1
				 || ((EMA1[5] > EMA2[5])
				 && (EMA3[5] > EMA4[5])
				 && (CrossAbove(EMA5, EMA6, 5))
				 && (EMA7[5] > EMA8[5])
				 && (Position.Quantity == Flat)
				 && (Times[0][0].TimeOfDay > StartTime.TimeOfDay)
				 && (Times[0][0].TimeOfDay < StopTime.TimeOfDay))
				 // Condition group 1
				 || ((EMA1[5] > EMA2[5])
				 && (EMA3[5] > EMA4[5])
				 && (EMA5[5] > EMA6[5])
				 && (CrossAbove(EMA7, EMA8, 5))
				 && (Position.Quantity == Flat)
				 && (Times[0][0].TimeOfDay > StartTime.TimeOfDay)
				 && (Times[0][0].TimeOfDay < StopTime.TimeOfDay)))
			{
				EnterLongLimit(Convert.ToInt32(DefaultQuantity), EMA9[0], @"Long1");
				EnterLongLimit(Convert.ToInt32(DefaultQuantity), EMA9[0], @"Long2");
			}
			
			 // Set 2
			if (
				 // Condition group 1
				((CrossBelow(EMA1, EMA2, 5))
				 && (EMA3[5] < EMA4[5])
				 && (EMA5[5] < EMA6[5])
				 && (EMA7[5] < EMA8[5])
				 && (Position.Quantity == Flat)
				 && (Times[0][0].TimeOfDay > StartTime.TimeOfDay)
				 && (Times[0][0].TimeOfDay < StopTime.TimeOfDay))
				 // Condition group 1
				 || ((EMA1[5] < EMA2[5])
				 && (CrossBelow(EMA3, EMA4, 5))
				 && (EMA5[5] < EMA6[5])
				 && (EMA7[5] < EMA8[5])
				 && (Position.Quantity == Flat)
				 && (Times[0][0].TimeOfDay > StartTime.TimeOfDay)
				 && (Times[0][0].TimeOfDay < StopTime.TimeOfDay))
				 // Condition group 1
				 || ((EMA1[5] < EMA2[5])
				 && (EMA3[5] < EMA4[5])
				 && (CrossBelow(EMA5, EMA6, 5))
				 && (EMA7[5] < EMA8[5])
				 && (Position.Quantity == Flat)
				 && (Times[0][0].TimeOfDay > StartTime.TimeOfDay)
				 && (Times[0][0].TimeOfDay < StopTime.TimeOfDay))
				 // Condition group 1
				 || ((EMA1[5] < EMA2[5])
				 && (EMA3[5] < EMA4[5])
				 && (EMA5[5] < EMA6[5])
				 && (CrossBelow(EMA7, EMA8, 5))
				 && (Position.Quantity == Flat)
				 && (Times[0][0].TimeOfDay > StartTime.TimeOfDay)
				 && (Times[0][0].TimeOfDay < StopTime.TimeOfDay)))
			{
				EnterShortLimit(Convert.ToInt32(DefaultQuantity), EMA9[0], @"Short1");
				EnterShortLimit(Convert.ToInt32(DefaultQuantity), EMA9[0], @"Short2");
			}
			
		}

		#region Properties
		[NinjaScriptProperty]
		[Range(4, int.MaxValue)]
		[Display(Name="LongSL", Order=1, GroupName="Parameters")]
		public int LongSL
		{ get; set; }

		[NinjaScriptProperty]
		[Range(4, int.MaxValue)]
		[Display(Name="LongPT", Order=2, GroupName="Parameters")]
		public int LongPT
		{ get; set; }
		
		[NinjaScriptProperty]
		[Range(4, int.MaxValue)]
		[Display(Name="LongPT2", Order=3, GroupName="Parameters")]
		public int LongPT2
		{ get; set; }

		[NinjaScriptProperty]
		[Range(4, int.MaxValue)]
		[Display(Name="ShortSL", Order=4, GroupName="Parameters")]
		public int ShortSL
		{ get; set; }
		
		[NinjaScriptProperty]
		[Range(4, int.MaxValue)]
		[Display(Name="ShortPT", Order=5, GroupName="Parameters")]
		public int ShortPT
		{ get; set; }
		
		[NinjaScriptProperty]
		[Range(4, int.MaxValue)]
		[Display(Name="ShortPT2", Order=6, GroupName="Parameters")]
		public int ShortPT2
		{ get; set; }
		
		[NinjaScriptProperty]
		[PropertyEditor("NinjaTrader.Gui.Tools.TimeEditorKey")]
		[Display(Name="StartTime", Order=7, GroupName="Parameters")]
		public DateTime StartTime
		{ get; set; }

		[NinjaScriptProperty]
		[PropertyEditor("NinjaTrader.Gui.Tools.TimeEditorKey")]
		[Display(Name="StopTime", Order=8, GroupName="Parameters")]
		public DateTime StopTime
		{ get; set; }
		#endregion

	}
}
