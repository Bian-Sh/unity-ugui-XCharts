/******************************************/
/*                                        */
/*     Copyright (c) 2018 monitor1394     */
/*     https://github.com/monitor1394     */
/*                                        */
/******************************************/

using System;
using System.Collections.Generic;
using UnityEngine;

namespace XCharts
{
    public static class VisualMapHelper
    {
        public static void AutoSetLineMinMax(VisualMap visualMap, Serie serie, XAxis xAxis, YAxis yAxis)
        {
            if (!IsNeedGradient(visualMap) || !visualMap.autoMinMax) return;
            float min = 0;
            float max = 0;
            switch (visualMap.direction)
            {
                case VisualMap.Direction.Default:
                case VisualMap.Direction.X:
                    min = xAxis.IsCategory() ? 0 : xAxis.runtimeMinValue;
                    max = xAxis.IsCategory() ? serie.dataCount : xAxis.runtimeMaxValue;
                    SetMinMax(visualMap, min, max);
                    break;
                case VisualMap.Direction.Y:
                    min = yAxis.IsCategory() ? 0 : yAxis.runtimeMinValue;
                    max = yAxis.IsCategory() ? serie.dataCount : yAxis.runtimeMaxValue;
                    SetMinMax(visualMap, min, max);
                    break;
            }
        }

        public static void SetMinMax(VisualMap visualMap, float min, float max)
        {
            if (visualMap.enable && (visualMap.min != min || visualMap.max != max))
            {
                //Debug.LogError("minmax:"+min+","+max);
                if (max >= min)
                {
                    visualMap.min = min;
                    visualMap.max = max;
                    //Debug.LogError("minmax2222:"+visualMap.min+","+visualMap.max);
                }
                else
                {
                    throw new Exception("SetMinMax:max < min:" + min + "," + max);
                }
            }
        }

        public static void GetLineGradientColor(VisualMap visualMap, float xValue, float yValue,
            out Color startColor, out Color toColor)
        {
            startColor = Color.clear;
            toColor = Color.clear;
            switch (visualMap.direction)
            {
                case VisualMap.Direction.Default:
                case VisualMap.Direction.X:
                    startColor = visualMap.IsPiecewise() ? visualMap.GetColor(xValue) : visualMap.GetColor(xValue - 1);
                    toColor = visualMap.IsPiecewise() ? startColor : visualMap.GetColor(xValue);
                    break;
                case VisualMap.Direction.Y:
                    startColor = visualMap.IsPiecewise() ? visualMap.GetColor(yValue) : visualMap.GetColor(yValue - 1);
                    toColor = visualMap.IsPiecewise() ? startColor : visualMap.GetColor(yValue);
                    break;
            }
        }

        internal static Color GetLineGradientColor(VisualMap visualMap, Vector3 pos, CoordinateChart chart, Axis axis, Color defaultColor)
        {
            float value = 0;
            switch (visualMap.direction)
            {
                case VisualMap.Direction.Default:
                case VisualMap.Direction.X:
                    var min = axis.runtimeMinValue;
                    var max = axis.runtimeMaxValue;
                    value = min + (pos.x - chart.coordinateX) / chart.coordinateWidth * (max - min);
                    break;
                case VisualMap.Direction.Y:
                    if (axis is YAxis)
                    {
                        var yAxis = chart.xAxises[axis.index];
                        min = yAxis.runtimeMinValue;
                        max = yAxis.runtimeMaxValue;
                    }
                    else
                    {
                        var yAxis = chart.yAxises[axis.index];
                        min = yAxis.runtimeMinValue;
                        max = yAxis.runtimeMaxValue;
                    }
                    value = min + (pos.y - chart.coordinateY) / chart.coordinateHeight * (max - min);
                    break;
            }
            var color = visualMap.GetColor(value);
            if (ChartHelper.IsClearColor(color)) return defaultColor;
            else return color;
        }

        internal static Color GetItemStyleGradientColor(ItemStyle itemStyle, Vector3 pos, CoordinateChart chart, Axis axis, Color defaultColor)
        {
            var min = axis.runtimeMinValue;
            var max = axis.runtimeMaxValue;
            var value = min + (pos.x - chart.coordinateX) / chart.coordinateWidth * (max - min);
            var rate = (value - min) / (max - min);
            var color = itemStyle.GetGradientColor(rate, defaultColor);
            if (ChartHelper.IsClearColor(color)) return defaultColor;
            else return color;
        }

        public static bool IsNeedGradient(VisualMap visualMap)
        {
            if (!visualMap.enable || visualMap.inRange.Count <= 0) return false;
            return true;
        }

        public static int GetDimension(VisualMap visualMap, int serieDataCount)
        {
            var dimension = visualMap.enable && visualMap.dimension >= 0 ? visualMap.dimension : serieDataCount - 1;
            if (dimension > serieDataCount - 1) dimension = serieDataCount - 1;
            return dimension;
        }
    }
}