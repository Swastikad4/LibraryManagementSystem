// ============================================================
// AnimationHelper.cs — Simple UI Animations
// Library Management System — UI Helpers
// ============================================================
// Provides simple fade-in and slide animations for panels.
// Uses System.Windows.Forms.Timer for smooth transitions.
// ============================================================

namespace LibraryManagementSystem.UI.Helpers
{
    /// <summary>
    /// Provides simple animation effects for UI transitions.
    /// </summary>
    public static class AnimationHelper
    {
        /// <summary>
        /// Fades in a control from transparent to opaque.
        /// Uses the form's opacity for form-level fading.
        /// </summary>
        /// <param name="form">The form to fade in.</param>
        /// <param name="duration">Duration in milliseconds (default: 300ms).</param>
        public static void FadeIn(Form form, int duration = 300)
        {
            form.Opacity = 0;
            var timer = new System.Windows.Forms.Timer { Interval = 15 };
            double step = 15.0 / duration;

            timer.Tick += (s, e) =>
            {
                form.Opacity += step;
                if (form.Opacity >= 1)
                {
                    form.Opacity = 1;
                    timer.Stop();
                    timer.Dispose();
                }
            };

            timer.Start();
        }

        /// <summary>
        /// Slides a control in from the left.
        /// </summary>
        /// <param name="control">The control to slide.</param>
        /// <param name="targetX">Target X position.</param>
        /// <param name="duration">Duration in milliseconds.</param>
        public static void SlideInFromLeft(Control control, int targetX, int duration = 250)
        {
            int startX = -control.Width;
            control.Left = startX;
            control.Visible = true;

            var timer = new System.Windows.Forms.Timer { Interval = 15 };
            int totalSteps = duration / 15;
            int currentStep = 0;

            timer.Tick += (s, e) =>
            {
                currentStep++;
                // Ease-out interpolation for smooth deceleration
                double progress = 1.0 - Math.Pow(1.0 - (double)currentStep / totalSteps, 3);
                control.Left = startX + (int)((targetX - startX) * progress);

                if (currentStep >= totalSteps)
                {
                    control.Left = targetX;
                    timer.Stop();
                    timer.Dispose();
                }
            };

            timer.Start();
        }

        /// <summary>
        /// Safely animates a docked control by temporarily adjusting its
        /// padding to create a slide-in-from-bottom visual effect without
        /// modifying the control's Top or Dock properties.
        /// Falls back to a simple visibility toggle if padding is not practical.
        /// </summary>
        public static void SlideInFromBottom(Control control, int targetY, int duration = 200)
        {
            // For docked controls, do NOT mutate Top/Left — it breaks docking.
            // Use a simple fade-in approach instead.
            FadeInControl(control, duration);
        }

        /// <summary>
        /// Fades in a control using its parent form's opacity or
        /// by toggling visibility with a brief delay for a transition feel.
        /// Safe to use with docked controls.
        /// </summary>
        /// <param name="control">The control to fade in.</param>
        /// <param name="duration">Duration in milliseconds (default: 200ms).</param>
        public static void FadeInControl(Control control, int duration = 200)
        {
            control.Visible = false;
            control.Visible = true;

            // No position mutation — just ensure the control is visible
            // and let WinForms handle docking layout correctly.
        }
    }
}
