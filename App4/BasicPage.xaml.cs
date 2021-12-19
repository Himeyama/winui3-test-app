using Windows.ApplicationModel.Core;
using Windows.UI;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;

namespace App4
{
    public class BasicPage : Page
    {
        internal StackPanel atb;
        internal StackPanel at;
        internal TextBlock att;

        public BasicPage()
        {
            ApplicationViewTitleBar titleBar = ApplicationView.GetForCurrentView().TitleBar;

            titleBar.ButtonBackgroundColor = Colors.Transparent;
            titleBar.ButtonInactiveBackgroundColor = Colors.Transparent;
        }

        internal void MicaTitle(StackPanel AppTitleBar, StackPanel AppTitle, TextBlock AppTitleText)
        {
            atb = AppTitleBar;
            at = AppTitle;
            att = AppTitleText;

            //デフォルトのタイトルバーを非表示
            var coreTitleBar = CoreApplication.GetCurrentView().TitleBar;
            coreTitleBar.ExtendViewIntoTitleBar = true;
            UpdateTitleBarLayout(coreTitleBar);

            // XAML要素をドラッグ可能な領域として設定
            Window.Current.SetTitleBar(at);

            // オーバーレイされたキャプションコントロールのサイズが変更された場合のハンドラーを登録
            // たとえば、アプリが別のDPIの画面に移動した場合
            coreTitleBar.LayoutMetricsChanged += CoreTitleBar_LayoutMetricsChanged;

            // タイトルバーの表示が変更されたときのハンドラーを登録
            // たとえば、タイトルバーがフルスクリーンモードで呼び出された場合
            coreTitleBar.IsVisibleChanged += CoreTitleBar_IsVisibleChanged;

            // ウィンドウがフォーカスを変更したときのハンドラーを登録
            Window.Current.Activated += Current_Activated;
        }

        internal void UpdateTitleBarLayout(CoreApplicationViewTitleBar coreTitleBar)
        {
            // システムサイズの変更に対応するために、必要に応じてタイトルバーのコントロールサイズを更新
            //atb.Height = coreTitleBar.Height;
            atb.Height = 60;

            // カスタムタイトルバーがウィンドウのキャプションコントロールと重ならないように
            Thickness currMargin = atb.Margin;
            atb.Margin = new Thickness(currMargin.Left, currMargin.Top, coreTitleBar.SystemOverlayRightInset, currMargin.Bottom);
        }

        internal void Current_Activated(object sender, Windows.UI.Core.WindowActivatedEventArgs e)
        {
            SolidColorBrush defaultForegroundBrush = (SolidColorBrush)Application.Current.Resources["TextFillColorPrimaryBrush"];
            SolidColorBrush inactiveForegroundBrush = (SolidColorBrush)Application.Current.Resources["TextFillColorDisabledBrush"];

            if (e.WindowActivationState == Windows.UI.Core.CoreWindowActivationState.Deactivated)
            {
                att.Foreground = inactiveForegroundBrush;
            }
            else
            {
                att.Foreground = defaultForegroundBrush;
            }
        }


        internal void CoreTitleBar_LayoutMetricsChanged(CoreApplicationViewTitleBar sender, object args)
        {
            UpdateTitleBarLayout(sender);
        }

        private void CoreTitleBar_IsVisibleChanged(CoreApplicationViewTitleBar sender, object args)
        {
            if (sender.IsVisible)
            {
                atb.Visibility = Visibility.Visible;
            }
            else
            {
                atb.Visibility = Visibility.Collapsed;
            }
        }
    }
}
