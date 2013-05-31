using System;
using System.Drawing;
using MonoTouch.UIKit;
using MonoTouch.Foundation;

namespace OasisMobile.iOS
{
	public class CustomImageCell : UITableViewCell
	{
		private float m_imageMaxDimension = 0;

		public CustomImageCell (string aReuseIdentifier) : base(UITableViewCellStyle.Default, aReuseIdentifier)
		{

		}

		public float MaxImageDimension {
			get {
				return m_imageMaxDimension;
			}
			set {
				m_imageMaxDimension = value;
			}
		}

		public override void LayoutSubviews ()
		{
			base.LayoutSubviews ();

			if (m_imageMaxDimension <= 0 || m_imageMaxDimension > ContentView.Frame.Width) {
				//If image max dimension does not exist, the max is the same as the content view width
				m_imageMaxDimension = ContentView.Frame.Width - 20;
			}

			UIImage _imageToDisplay = ImageView.Image;
			SizeF _imageFileDimension;
			float _imageWidthToHeightRatio;
			SizeF _imageViewDimension;
			PointF _imageViewPosition;

			if (_imageToDisplay != null) {
				_imageFileDimension = _imageToDisplay.Size;
				_imageWidthToHeightRatio = _imageFileDimension.Width / _imageFileDimension.Height;
				if (_imageWidthToHeightRatio >= 1) {
					_imageViewDimension = new SizeF (m_imageMaxDimension, m_imageMaxDimension / _imageWidthToHeightRatio);
				} else {
					_imageViewDimension = new SizeF (_imageWidthToHeightRatio * m_imageMaxDimension, m_imageMaxDimension);
				}
				_imageViewPosition = new PointF (ContentView.Frame.Width/2 - _imageViewDimension.Width/2, 10);
			} else {
				_imageFileDimension = new SizeF (0, 0);
				_imageWidthToHeightRatio = 1;
				_imageViewDimension = new SizeF (0, 0);
				_imageViewPosition = new PointF (0, 0);
			}


			ImageView.Frame = new RectangleF (_imageViewPosition, _imageViewDimension);

			if (TextLabel.Text != null && TextLabel.Text != "") {
				SizeF _labelDimension;
				PointF _labelPosition;

				TextLabel.TextAlignment = UITextAlignment.Center;
				_labelDimension = this.StringSize (TextLabel.Text, TextLabel.Font, 
				                                   new SizeF (ContentView.Frame.Width-20, float.MaxValue), UILineBreakMode.WordWrap);
				_labelPosition = new PointF ();
				_labelPosition.X = ContentView.Frame.Width / 2 - _imageViewDimension.Width / 2;
				_labelPosition.Y = _imageViewPosition.Y + _imageFileDimension.Height + 5;

				TextLabel.Frame = new RectangleF (_labelPosition, _labelDimension);
			}
		}
	}
}

