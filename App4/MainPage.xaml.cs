using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text.RegularExpressions;
using Windows.Foundation;
using Windows.Globalization;
using Windows.Graphics.Imaging;
using Windows.Media.Ocr;
using Windows.Storage;
using Windows.Storage.Pickers;
using Windows.Storage.Streams;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Documents;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;

namespace App4
{
    public sealed partial class MainPage : BasicPage
    {
        string sentence;
        IReadOnlyList<Language> langs;
        StorageFile file;

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
            SelectLang.SelectedIndex = 0;
        }

        private async void OpenFile(object sender, RoutedEventArgs e)
        {
            FileOpenPicker filePicker = new();
            filePicker.FileTypeFilter.Add(".png");
            file = await filePicker.PickSingleFileAsync();
            //string fileName = file.Path;

            if (file != null)
            {
                using (IRandomAccessStream fileStream = await file.OpenAsync(Windows.Storage.FileAccessMode.Read))
                {
                    BitmapImage bitmapImage = new();
                    await bitmapImage.SetSourceAsync(fileStream);
                    inputImage.Source = bitmapImage;
                }

                SelectLang_SelectionChanged(sender, e);
            }
        }

        private async void SelectLang_SelectionChanged(object sender, RoutedEventArgs e)
        {
            if (file != null)
            {
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
                text.Text = "";

                Regex regex = new("[a-zA-Z]+|.+?");
                MatchCollection words = regex.Matches(sentence);

                foreach (Match word in words)
                {
                    string sWord = word.Value;
                    Run run = new();
                    if (Regex.IsMatch(sWord, "[a-zA-Z]+"))
                    {
                        Hyperlink hyperlink = new Hyperlink();
                        //hyperlink.NavigateUri = uri;
                        run.Text = sWord;
                        hyperlink.Inlines.Add(run);
                        hyperlink.Click += OpenLink;
                        hyperlink.AccessKey = sWord;

                        text.Inlines.Add(hyperlink);
                    }
                    else
                    {
                        run.Text = sWord;
                        text.Inlines.Add(run);
                    }
                }
            }
        }

        private void OpenLink(Hyperlink sender, HyperlinkClickEventArgs args)
        {
            Uri uri = new($"https://translate.google.co.jp/?hl=ja&sl=en&tl=ja&text={sender.AccessKey}");
            wv2.Navigate(uri);
        }

        private void ExitApp(object sender, RoutedEventArgs e)
        {
            Application.Current.Exit();
        }
    }
}
