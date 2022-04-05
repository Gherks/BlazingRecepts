﻿using BlazingRecept.Server.Entities.Bases;
using System.ComponentModel.DataAnnotations.Schema;

namespace BlazingRecept.Server.Entities;

public sealed class Recipe : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public string Instructions { get; set; } = string.Empty;
    public int PortionAmount { get; set; } = -1;
    public Guid CategoryId { get; set; } = Guid.Empty;
    public List<IngredientMeasurement> IngredientMeasurements { get; set; } = new();
}
