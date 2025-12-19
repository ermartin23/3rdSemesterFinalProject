using System;

using api.Helpers.Time;

namespace tests.ApiTests;
public class FakeClock : IClock
{ 
    public DateTime UtcNow { get; set; } = DateTime.UtcNow;
}