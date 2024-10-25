using System.Drawing;
using System.Windows.Forms;

namespace SpeedMesApp
{
  /// <summary>
  /// User control for entering time and speed data.
  /// </summary>
  public partial class MeasurementInputControl : UserControl
  {
    public Label lblTime;
    public Label lblSpeed; 
    public TextBox txtTime; 
    public TextBox txtSpeed;

    public MeasurementInputControl()
    {
      lblTime = new Label() { Text = "Idő (s):", Location = new Point(10, 10), AutoSize = true };
      txtTime = new TextBox() { Location = new Point(80, 7), Width = 100 };
      lblSpeed = new Label() { Text = "Sebesség (m/s):", Location = new Point(200, 10), AutoSize = true };
      txtSpeed = new TextBox() { Location = new Point(300, 7), Width = 100 };

      this.Controls.Add(lblTime);
      this.Controls.Add(txtTime);
      this.Controls.Add(lblSpeed);
      this.Controls.Add(txtSpeed);

      this.Height = 35;
      this.Width = 420;
    }

    /// <summary>
    /// Attempts to create the Measurement object from the input data.
    /// </summary>
    /// <param name="measurement">The Measurement object created if the input was successful.</param>
    /// <returns>True if the input is valid and the Measurement is created; otherwise false.</returns>
    public bool TryGetMeasurement(out Measurement measurement) // TryGetMeres
    {
      measurement = null;
      if (double.TryParse(txtTime.Text, out double time) && double.TryParse(txtSpeed.Text, out double speed))
      {
        if (time >= 0 && speed >= 0)
        {
          measurement = new Measurement(time, speed);
          return true;
        }
      }
      return false;
    }

    /// <summary>
    /// Deletes the content of the text fields for entering time and speed.
    /// </summary>
    public void ClearInputs()
    {
      txtTime.Clear();
      txtSpeed.Clear();
    }
  }
}
