using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace AsyncAwaitWithMVVM
{
  public partial class CircularBusyIndicator : UserControl
  {
    private readonly DispatcherTimer animationTimer;

    public bool IsBusy
    {
      get { return (bool)GetValue(IsBusyProperty); }
      set { SetValue(IsBusyProperty, value); }
    }

    public static readonly DependencyProperty IsBusyProperty = 
      DependencyProperty.Register("IsBusy", typeof(bool), typeof(CircularBusyIndicator),
        new UIPropertyMetadata(false, new PropertyChangedCallback(OnIsBusyPropertyChanged)));

    private static void OnIsBusyPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
      if(d is CircularBusyIndicator circularBusyIndicator)
      {
        if(circularBusyIndicator.IsBusy)
        {
          circularBusyIndicator.Visibility = Visibility.Visible;
          circularBusyIndicator.StartAnimation();
        }
        else
        {
          circularBusyIndicator.Visibility = Visibility.Collapsed;
          circularBusyIndicator.Stop();
        }
      }
    }

    private void StartAnimation()
    {
      const double offset = Math.PI;
      const double step = Math.PI * 2 / 10.0;

      SetPosition(C0, offset, 0.0, step);
      SetPosition(C1, offset, 1.0, step);
      SetPosition(C2, offset, 2.0, step);
      SetPosition(C3, offset, 3.0, step);
      SetPosition(C4, offset, 4.0, step);
      SetPosition(C5, offset, 5.0, step);
      SetPosition(C6, offset, 6.0, step);
      SetPosition(C7, offset, 7.0, step);
      SetPosition(C8, offset, 8.0, step);

      Start();
    }

    public CircularBusyIndicator()
    {
      InitializeComponent();

      animationTimer = new DispatcherTimer(DispatcherPriority.ContextIdle, Dispatcher);
      animationTimer.Interval = TimeSpan.FromMilliseconds(100);
    }

    private void Start()
    {
      animationTimer.Tick += HandleAnimationTick;
      animationTimer.Start();
    }

    private void Stop()
    {
      animationTimer.Stop();
      animationTimer.Tick -= HandleAnimationTick;
    }

    private void HandleAnimationTick(object sender, EventArgs e)
    {
      SpinnerRotate.Angle = (SpinnerRotate.Angle + 36) % 360;
    }

    private void HandleLoaded(object sender, RoutedEventArgs e)
    {
      const double offset = Math.PI;
      const double step = Math.PI * 2 / 10.0;

      SetPosition(C0, offset, 0.0, step);
      SetPosition(C1, offset, 1.0, step);
      SetPosition(C2, offset, 2.0, step);
      SetPosition(C3, offset, 3.0, step);
      SetPosition(C4, offset, 4.0, step);
      SetPosition(C5, offset, 5.0, step);
      SetPosition(C6, offset, 6.0, step);
      SetPosition(C7, offset, 7.0, step);
      SetPosition(C8, offset, 8.0, step);

      Start();
    }

    private void SetPosition(Ellipse ellipse, double offset, double posOffSet, double step)
    {
      ellipse.SetValue(Canvas.LeftProperty, 50.0 + Math.Sin(offset + posOffSet * step) * 50.0);
      ellipse.SetValue(Canvas.TopProperty, 50 + Math.Cos(offset + posOffSet * step) * 50.0);
    }

    private void HandleUnloaded(object sender, RoutedEventArgs e)
    {
      Stop();
    }
  }
}
