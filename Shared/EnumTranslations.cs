﻿using static BlazingRecept.Shared.Enums;

namespace BlazingRecept.Shared;

public static class EnumTranslations
{
    public static string ToSymbol(this MeasurementUnit measurementUnit)
    {
        switch (measurementUnit)
        {
            case MeasurementUnit.Gram:
                return "g";
            case MeasurementUnit.Hectogram:
                return "hg";
            case MeasurementUnit.Kilogram:
                return "kg";
            case MeasurementUnit.SpiceMeasurement:
                return "krm";
            case MeasurementUnit.Teaspoon:
                return "tsk";
            case MeasurementUnit.Tablespoon:
                return "msk";
            case MeasurementUnit.Milliliters:
                return "ml";
            case MeasurementUnit.Centiliters:
                return "cl";
            case MeasurementUnit.Deciliters:
                return "dl";
            case MeasurementUnit.Liter:
                return "l";
            default:
                throw new InvalidOperationException("Unknown measurement given.");
        }
    }
}