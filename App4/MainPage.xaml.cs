using System;
using System.Collections.Generic;
using System.Diagnostics;
using Windows.Globalization;
using Windows.Graphics.Imaging;
using Windows.Media.Ocr;
using Windows.Storage;
using Windows.Storage.Pickers;
using Windows.Storage.Streams;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;

namespace App4
{
    public sealed partial class MainPage : BasicPage
    {
        string sentence;
        IReadOnlyList<Language> langs;

        public MainPage(): base()
        {
            InitializeComponent();
            MicaTitle(AppTitleBar, AppTitle, AppTitleText);
            //Log.debug(OcrEngine.AvailableRecognizerLanguages);

            langs = OcrEngine.AvailableRecognizerLanguages;
            for(int i = 0; i < langs.Count; i++)
            {
                Language language = langs[i];
                // English (United States)
                string langNativeName = language.NativeName;

                // en-US
                string lang = language.LanguageTag;

                SelectLang.Items.Add(langNativeName);
            }
        }

        private async void OpenFile(object sender, RoutedEventArgs e)
        {
            FileOpenPicker filePicker = new();
            filePicker.FileTypeFilter.Add(".png");
            StorageFile file = await filePicker.PickSingleFileAsync();
            //string fileName = file.Path;

            if (file != null)
            {
                using (IRandomAccessStream fileStream = await file.OpenAsync(Windows.Storage.FileAccessMode.Read))
                {
                    BitmapImage bitmapImage = new();
                    await bitmapImage.SetSourceAsync(fileStream);
                    inputImage.Source = bitmapImage;
                }

                // 言語
                Language lang = langs[SelectLang.SelectedIndex];


                // OCR
                SoftwareBitmap softwareBitmap;
                using (IRandomAccessStream stream = await file.OpenAsync(FileAccessMode.Read))
                {
                    BitmapDecoder decoder = await BitmapDecoder.CreateAsync(stream);
                    softwareBitmap = await decoder.GetSoftwareBitmapAsync();
                }
                //OcrEngine ocrEngine = OcrEngine.TryCreateFromUserProfileLanguages();
                OcrEngine ocrEngine = OcrEngine.TryCreateFromLanguage(lang);
                OcrResult result = await ocrEngine.RecognizeAsync(softwareBitmap);
                sentence = result.Text;
                text.Text = sentence;
            }
        }

        private void ExitApp(object sender, RoutedEventArgs e)
        {
            Application.Current.Exit();
        }
    }
}
