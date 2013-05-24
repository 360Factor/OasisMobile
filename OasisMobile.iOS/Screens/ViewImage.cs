using System;
using System.Drawing;
using MonoTouch.Foundation;
using MonoTouch.UIKit;
using System.Collections.Generic;
using System.Linq;

namespace OasisMobile.iOS
{
	public partial class ViewImage : UIViewController
	{
		private List<BusinessModel.Image> m_questionImagesList;
		private int m_currentImageDisplayIndex;
		private UIScrollView svCurrentImageZoomView;
		private UIScrollView svNextImageZoomView;
		private UIScrollView svPreviousImageZoomView;

		static bool UserInterfaceIdiomIsPhone {
			get { return UIDevice.CurrentDevice.UserInterfaceIdiom == UIUserInterfaceIdiom.Phone; }
		}

		public ViewImage (int aImageToDisplayID)
			: base (UserInterfaceIdiomIsPhone ? "ViewImage_iPhone" : "ViewImage_iPad", null)
		{
			m_questionImagesList = BusinessModel.Image.GetImagesBySQL ( string.Format (
								    "SELECT tblImageToReturn.* FROM tblImage AS tblImageToReturn INNER JOIN tblImage AS tblOriginalImage " +
								    "ON tblImageToReturn.fkQuestionID = tblOriginalImage.fkQuestionID " +
									"WHERE tblOriginalImage.pkImageID={0}",aImageToDisplayID));
			m_currentImageDisplayIndex = m_questionImagesList.FindIndex (x => x.ImageID==aImageToDisplayID);
		}

		public override void DidReceiveMemoryWarning ()
		{
			// Releases the view if it doesn't have a superview.
			base.DidReceiveMemoryWarning ();
			
			// Release any cached data, images, etc that aren't in use.
		}

		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();

			RectangleF _scrollViewFrame = svImagePager.Frame;
			svImagePager.ContentSize = new SizeF( _scrollViewFrame.Width * m_questionImagesList.Count, _scrollViewFrame.Height);
			svImagePager.AutoresizingMask = UIViewAutoresizing.All;

			//Load the current Image
			BusinessModel.Image _imageRecord = m_questionImagesList [m_currentImageDisplayIndex];
			UIImage _imageDataToDisplay = UIImage.FromFile (_imageRecord.FilePath);

			UIImageView _imageViewToDisplay = new UIImageView (_imageDataToDisplay);
			_imageViewToDisplay.AutoresizingMask = UIViewAutoresizing.All;
			_imageViewToDisplay.Frame = new RectangleF (0, 0, _imageDataToDisplay.Size.Width, _imageDataToDisplay.Size.Height);

			RectangleF _imageScrollViewFrame = this.View.Frame;
			svCurrentImageZoomView= new UIScrollView (_imageScrollViewFrame);

			svCurrentImageZoomView.ContentSize = _imageDataToDisplay.Size;
			svCurrentImageZoomView.AddSubview (_imageViewToDisplay);
			svCurrentImageZoomView.ViewForZoomingInScrollView = GetZoomedView;
			svCurrentImageZoomView.DidZoom += (object sender, EventArgs e) => {
				CenterScrollViewImage ();
			};
			svImagePager.AddSubview (svCurrentImageZoomView);
			svImagePager.SetContentOffset (new PointF(_scrollViewFrame.Width * m_currentImageDisplayIndex,0),
			                              false);
			// Perform any additional setup after loading the view, typically from a nib.
		}

		public UIView GetZoomedView (UIScrollView aZoomedScrollView){
			return aZoomedScrollView.Subviews[0];
		}

		public override void ViewWillAppear (bool animated)
		{
			base.ViewWillAppear (animated);
			SetDefaultScrollViewZoom ();
			CenterScrollViewImage ();

		}

		private void SetDefaultScrollViewZoom(){
			if (svCurrentImageZoomView != null) {
				float _widthScale = svCurrentImageZoomView.Frame.Width / svCurrentImageZoomView.ContentSize.Width;
				float _heightScale = svCurrentImageZoomView.Frame.Height / svCurrentImageZoomView.ContentSize.Height;
				float _minScale = Math.Min (_widthScale, _heightScale);
				svCurrentImageZoomView.MinimumZoomScale = _minScale;
				svCurrentImageZoomView.MaximumZoomScale = 1;
				svCurrentImageZoomView.ZoomScale = _minScale;
			}

			if (svPreviousImageZoomView != null) {
				float _widthScale = svPreviousImageZoomView.Frame.Width / svPreviousImageZoomView.ContentSize.Width;
				float _heightScale = svPreviousImageZoomView.Frame.Height / svPreviousImageZoomView.ContentSize.Height;
				float _minScale = Math.Min (_widthScale, _heightScale);
				svPreviousImageZoomView.MinimumZoomScale = _minScale;
				svPreviousImageZoomView.MaximumZoomScale = 1;
				svPreviousImageZoomView.ZoomScale = _minScale;
			}

			if (svNextImageZoomView != null) {
				float _widthScale = svNextImageZoomView.Frame.Width / svNextImageZoomView.ContentSize.Width;
				float _heightScale = svNextImageZoomView.Frame.Height / svNextImageZoomView.ContentSize.Height;
				float _minScale = Math.Min (_widthScale, _heightScale);
				svNextImageZoomView.MinimumZoomScale = _minScale;
				svNextImageZoomView.MaximumZoomScale = 1;
				svNextImageZoomView.ZoomScale = _minScale;
			}
		}

		private void CenterScrollViewImage(){
			if (svCurrentImageZoomView != null) {
				SizeF _scrollViewBound = svCurrentImageZoomView.Bounds.Size;
				UIImageView _childImageView = (UIImageView) svCurrentImageZoomView.Subviews [0];
				RectangleF _contentsFrame = _childImageView.Frame;
				if (_contentsFrame.Width < _scrollViewBound.Width) {
					_contentsFrame.X = (_scrollViewBound.Width - _contentsFrame.Width) / (float)2;
				} else {
					_contentsFrame.X = 0;
				}

				if (_contentsFrame.Height < _scrollViewBound.Height) {
					_contentsFrame.Y = (_scrollViewBound.Height - _contentsFrame.Height) / (float)2;
				} else {
					_contentsFrame.Y = 0;
				}

				_childImageView.Frame = _contentsFrame;
			}

			if (svPreviousImageZoomView != null) {
				SizeF _scrollViewBound = svPreviousImageZoomView.Bounds.Size;
				UIImageView _childImageView = (UIImageView) svPreviousImageZoomView.Subviews [0];
				RectangleF _contentsFrame = _childImageView.Frame;
				if (_contentsFrame.Width < _scrollViewBound.Width) {
					_contentsFrame.X = (_scrollViewBound.Width - _contentsFrame.Width) / (float)2;
				} else {
					_contentsFrame.X = 0;
				}

				if (_contentsFrame.Height < _scrollViewBound.Height) {
					_contentsFrame.Y = (_scrollViewBound.Height - _contentsFrame.Height) / (float)2;
				} else {
					_contentsFrame.Y = 0;
				}

				_childImageView.Frame = _contentsFrame;
			}

			if (svNextImageZoomView != null) {
				SizeF _scrollViewBound = svNextImageZoomView.Bounds.Size;
				UIImageView _childImageView = (UIImageView) svNextImageZoomView.Subviews [0];
				RectangleF _contentsFrame = _childImageView.Frame;
				if (_contentsFrame.Width < _scrollViewBound.Width) {
					_contentsFrame.X = (_scrollViewBound.Width - _contentsFrame.Width) / (float)2;
				} else {
					_contentsFrame.X = 0;
				}

				if (_contentsFrame.Height < _scrollViewBound.Height) {
					_contentsFrame.Y = (_scrollViewBound.Height - _contentsFrame.Height) / (float)2;
				} else {
					_contentsFrame.Y = 0;
				}

				_childImageView.Frame = _contentsFrame;
			}
		}

	}
}

