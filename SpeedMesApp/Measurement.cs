using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpeedMesApp
{

  /// <summary>
  /// Representation of time and speed data of a single measurement.
  /// </summary>
  public class Measurement
  {
    public double Time { get; set; }
    public double Speed { get; set; }

    /// <summary>
    /// Creates a new Measurement instance with the specified time and speed.
    /// </summary>
    public Measurement(double time, double speed)
    {
      Time = time;
      Speed = speed;
    }
  }

}
