using OpenCvSharp;
using OpenCvSharp.WpfExtensions;
using System;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Windows.Controls;
using System.Windows.Media.Imaging;

namespace WpfControlLibrary1
{
    /// <summary>
    /// Interaction logic for UserControl1.xaml
    /// </summary>
    public partial class CVImage : UserControl
    {
        public CVImage()
        {
            InitializeComponent();
        }

        private Subject<Mat> CvImageSubject = new Subject<Mat>();
        public void CvImageSubscribe(Action<Mat> onNext)
        {
            _ = CvImageSubject.AsObservable().Subscribe(onNext);
        }

        private Mat _CvImage;

        public Mat CvImage
        {
            set
            {
                _CvImage = value;
                BitmapSource bmpsrc = BitmapSourceConverter.ToBitmapSource(value);
                image.Source = bmpsrc;

                CvImageSubject.OnNext(_CvImage);
            }
            get
            {
                return _CvImage;
            }
        }
    }
}