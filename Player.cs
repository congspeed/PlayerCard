using System.Collections.Generic;

public class Player
{
    public string Name { get; set; }
    public string Team { get; set; }
    public string PhotoPath { get; set; }
    public double? PointsPerGame { get; set; }
    public double? Rebounds { get; set; }
    public double? Assists { get; set; }
    public double? ShootingPercentage { get; set; }
    public List<string> Achievements { get; set; } = new List<string>();
}
