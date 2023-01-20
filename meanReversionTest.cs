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
	public class meanReversionTest : Strategy
	{
		private RSI RSI1;
		private HMA HMA1;
		private HMA HMA2;
		
		private EMA EMA1;
		private TSSuperTrend TSSuperTrend1;

		protected override void OnStateChange()
		{
			if (State == State.SetDefaults)
			{
				Description									= @"Enter the description for your new custom Strategy here.";
				Name										= "meanReversionTest";
				Calculate									= Calculate.OnEachTick;
				EntriesPerDirection							= 1;
				EntryHandling								= EntryHandling.AllEntries;
				IsExitOnSessionCloseStrategy				= true;
				ExitOnSessionCloseSeconds					= 30;
				IsFillLimitOnTouch							= false;
				MaximumBarsLookBack							= MaximumBarsLookBack.TwoHundredFiftySix;
				OrderFillResolution							= OrderFillResolution.Standard;
				Slippage									= 0;
				StartBehavior								= StartBehavior.WaitUntilFlat;
				TimeInForce									= TimeInForce.Day;
				TraceOrders									= false;
				RealtimeErrorHandling						= RealtimeErrorHandling.StopCancelClose;
				StopTargetHandling							= StopTargetHandling.PerEntryExecution;
				BarsRequiredToTrade							= 20;
				// Disable this property for performance gains in Strategy Analyzer optimizations
				// See the Help Guide for additional information
				IsInstantiatedOnEachOptimizationIteration	= true;
				LongSL										= 60;
				LongPT										= 60;
				LongPT2										= 100;
				ShortSL										= 60;
				ShortPT										= 60;
				ShortPT2									= 100;
				rsiOversold									= 30;
				rsiOverbought								= 70;
				rsiPeriod									= 14;
				rsiSmooth									= 3;
				hma1Length									= 14;
				hma2Length									= 30;
				
			}
			else if (State == State.Configure)
			{
				AddDataSeries(Data.BarsPeriodType.Minute, 60);
				AddDataSeries(Data.BarsPeriodType.Minute, 5);
			}
			else if (State == State.DataLoaded)
			{				
				RSI1				= RSI(Close, rsiPeriod, rsiSmooth);
				HMA1				= HMA(Close, 14);
				HMA2				= HMA(Close, 30);
				EMA1				= EMA(Closes[2], 9);
				TSSuperTrend1				= TSSuperTrend(Closes[1], SuperTrendMode.ATR, 14, 2.618, MovingAverageType.HMA, 14, false, false, false);
				RSI1.Plots[0].Brush = Brushes.DodgerBlue;
				RSI1.Plots[1].Brush = Brushes.Goldenrod;
				HMA1.Plots[0].Brush = Brushes.Fuchsia;
				HMA2.Plots[0].Brush = Brushes.Goldenrod;
				AddChartIndicator(RSI1);
				AddChartIndicator(HMA1);
				AddChartIndicator(HMA2);
				
				//Entries
				SetProfitTarget(@"long1", CalculationMode.Ticks, LongPT);
				SetStopLoss(@"long1", CalculationMode.Ticks, LongSL, false);
				SetProfitTarget(@"long2", CalculationMode.Ticks, LongPT2);
				SetStopLoss(@"long2", CalculationMode.Ticks, LongSL, false);
				SetProfitTarget(@"short1", CalculationMode.Ticks, ShortPT);
				SetStopLoss(@"short1", CalculationMode.Ticks, ShortSL, false);			
				SetProfitTarget(@"short2", CalculationMode.Ticks, ShortPT2);
				SetStopLoss(@"short2", CalculationMode.Ticks, ShortSL, false);
			}
		}

		protected override void OnBarUpdate()
		{
			if (BarsInProgress != 0) 
				return;

			if (CurrentBars[0] < 13
			|| CurrentBars[1] < 0
			|| CurrentBars[2] < 0)
				return;

			 // Set 1
			if ((CrossBelow(RSI1.Default, rsiOverbought, 13))
				 && (CrossBelow(HMA1, HMA2, 1))
				 && (Times[0][0].TimeOfDay > new TimeSpan(9, 45, 0))
				 && (Times[0][0].TimeOfDay < new TimeSpan(15, 0, 0))
				 && (Closes[2][0] < EMA1[0]))
			{
				EnterShort(Convert.ToInt32(DefaultQuantity), @"short1");
				EnterShort(Convert.ToInt32(DefaultQuantity), @"short2");
			}
			
			 // Set 2
			if ((CrossAbove(RSI1.Default, rsiOversold, 13))
				 && (CrossAbove(HMA1, HMA2, 1))
				 && (Times[0][0].TimeOfDay > new TimeSpan(9, 45, 0))
				 && (Times[0][0].TimeOfDay < new TimeSpan(15, 0, 0))
				 && (TSSuperTrend1.UpTrend[0] > 0))
			{
				EnterLong(Convert.ToInt32(DefaultQuantity), @"long1");
				EnterLong(Convert.ToInt32(DefaultQuantity), @"long2");
			}
			
		}
		
		#region Properties
		[NinjaScriptProperty]
		[Range(1, int.MaxValue)]
		[Display(Name="LongSL", Order=1, GroupName="Parameters")]
		public int LongSL
		{ get; set; }

		[NinjaScriptProperty]
		[Range(1, int.MaxValue)]
		[Display(Name="LongPT", Order=2, GroupName="Parameters")]
		public int LongPT
		{ get; set; }
		
		[NinjaScriptProperty]
		[Range(1, int.MaxValue)]
		[Display(Name="LongPT2", Order=3, GroupName="Parameters")]
		public int LongPT2
		{ get; set; }

		[NinjaScriptProperty]
		[Range(1, int.MaxValue)]
		[Display(Name="ShortSL", Order=4, GroupName="Parameters")]
		public int ShortSL
		{ get; set; }
		
		[NinjaScriptProperty]
		[Range(1, int.MaxValue)]
		[Display(Name="ShortPT", Order=5, GroupName="Parameters")]
		public int ShortPT
		{ get; set; }
		
		[NinjaScriptProperty]
		[Range(1, int.MaxValue)]
		[Display(Name="ShortPT2", Order=6, GroupName="Parameters")]
		public int ShortPT2
		{ get; set; }
		
		[NinjaScriptProperty]
		[Range(1, int.MaxValue)]
		[Display(Name="rsiOversold", Order=7, GroupName="Parameters")]
		public int rsiOversold
		{ get; set; }

		[NinjaScriptProperty]
		[Range(1, int.MaxValue)]
		[Display(Name="rsiOverbought", Order=8, GroupName="Parameters")]
		public int rsiOverbought
		{ get; set; }
		
		[NinjaScriptProperty]
		[Range(1, int.MaxValue)]
		[Display(Name="rsiPeriod", Order=9, GroupName="Parameters")]
		public int rsiPeriod
		{ get; set; }

		[NinjaScriptProperty]
		[Range(1, int.MaxValue)]
		[Display(Name="rsiSmooth", Order=10, GroupName="Parameters")]
		public int rsiSmooth
		{ get; set; }
		
		[NinjaScriptProperty]
		[Range(1, int.MaxValue)]
		[Display(Name="hma1Length", Order=11, GroupName="Parameters")]
		public int hma1Length
		{ get; set; }
		
		[NinjaScriptProperty]
		[Range(1, int.MaxValue)]
		[Display(Name="hma2Length", Order=12, GroupName="Parameters")]
		public int hma2Length
		{ get; set; }
		
		#endregion
	}
}
