using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace SourceChord.FluentWPF
{
    public class ComboBoxItemVisualStateManager : VisualStateManager
    {
        private const string Normal = "Normal";
        internal const string PointerOver = "PointerOver";
        internal const string KeyboardFocus = "KeyboardFocus";
        private const string Disabled = "Disabled";
        internal const string Pressed = "Pressed";
        private const string Selected = "Selected";
        private const string SelectedDisabled = "SelectedDisabled";
        private const string SelectedPointerOver = "SelectedPointerOver";
        private const string SelectedPressed = "SelectedPressed";

        protected override bool GoToStateCore(FrameworkElement control, FrameworkElement stateGroupsRoot, string stateName, VisualStateGroup group, VisualState state, bool useTransitions)
        {
            if (group is null || state is null)
            {
                return false;
            }

            var item = (ComboBoxItem)control;

            // If no state is defined yet, go to Normal without additional checks
            if (group.CurrentState == null)
            {
                var normal = group.States.OfType<VisualState>().Single(s => s.Name == Normal);
                return base.GoToStateCore(item, stateGroupsRoot, Normal, group, normal, false);
            }

            var status = new
            {
                Disabled = !item.IsEnabled,
                //KeyboardFocus = item.IsKeyboardFocusWithin,
                PointerOver = item.IsMouseOver || item.IsStylusOver,
                Pressed = stateName == Pressed,
                Selected = item.IsSelected
            };

            var nextStateName = status switch
            {
                { Disabled: true, Selected: true } => SelectedDisabled,
                { Disabled: true } => Disabled,
                { Pressed: true, Selected: true } => SelectedPressed,
                { Pressed: true } => Pressed,
                { PointerOver: true, Selected: true } => SelectedPointerOver,
                { PointerOver: true } => PointerOver,
                //{ KeyboardFocus: true } => KeyboardFocus,
                { Selected: true } => Selected,
                _ => Normal
            };

            // Avoid transition from state to itself (which would produce a false result either way)
            if (group.CurrentState.Name == nextStateName)
            {
                return false;
            }

            var nextState = group.States.OfType<VisualState>().Single(s => s.Name == nextStateName);
            return base.GoToStateCore(item, stateGroupsRoot, nextStateName, group, nextState, false);
        }
    }

    public partial class ComboBoxResourceDictionary : ResourceDictionary
    {
        private void ComboBoxItem_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            // Prevent Pressed visual state with touch devices
            if (e.StylusDevice != null)
            {
                VisualStateManager.GoToState((ComboBoxItem)sender, ComboBoxItemVisualStateManager.Pressed, false);
            }
        }

        /// <summary>
        /// Handles the MouseEnter event on a <see cref="ComboBoxItem"/>.
        /// It is required since the hovering is not always handled properly with the modified visual state.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void LayoutRoot_MouseEnter(object sender, MouseEventArgs e)
        {
            // Prevent PointerOver visual state with touch devices
            if (e.StylusDevice == null)
            {
                var item = ((FrameworkElement)sender).TemplatedParent;
                VisualStateManager.GoToState((ComboBoxItem)item, ComboBoxItemVisualStateManager.PointerOver, false);
            }
        }
    }
}
