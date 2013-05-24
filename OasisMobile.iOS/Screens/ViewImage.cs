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






			svImagePager.Scrolled += svImagePager_Scrolled;
			// Perform any additional setup after loading the view, typically from a nib.
		}


		public void DisplayCurrentImageInScrollView(){
			//Load the current Image
			//-------------------------
			BusinessModel.Image _currentImageRecord = m_questionImagesList [m_currentImageDisplayIndex];
			UIImage _currentImageDataToDisplay = UIImage.FromFile (_currentImageRecord.FilePath);

			UIImageView _currentImageViewToDisplay = new UIImageView (_currentImageDataToDisplay);
			_currentImageViewToDisplay.AutoresizingMask = UIViewAutoresizing.All;
			_currentImageViewToDisplay.Frame = new RectangleF (0, 0, _currentImageDataToDisplay.Size.Width, _currentImageDataToDisplay.Size.Height);

			RectangleF _currentImageScrollViewFrame = svImagePager.Frame;
			_currentImageScrollViewFrame.X = _currentImageScrollViewFrame.Width * m_currentImageDisplayIndex;
			svCurrentImageZoomView= new UIScrollView (_currentImageScrollViewFrame);

			svCurrentImageZoomView.ContentSize = _currentImageDataToDisplay.Size;
			svCurrentImageZoomView.AddSubview (_currentImageViewToDisplay);
			svCurrentImageZoomView.ViewForZoomingInScrollView = GetZoomedView;
			svCurrentImageZoomView.DidZoom += (object sender, EventArgs e) => {
				CenterCurrentScrollViewImages ();
			};

			if (m_currentImageDisplayIndex) {

			}
			else{

			}

			svImagePager.AddSubview (svCurrentImageZoomView);

			svImagePager.SetContentOffset (new PointF(_currentImageScrollViewFrame.Width * m_currentImageDisplayIndex,0),
			                               false);
		}

		public override void ViewDidLayoutSubviews ()
		{
			base.ViewDidLayoutSubviews ();

			RectangleF _scrollViewFrame = svImagePager.Frame;
			svImagePager.ContentSize = new SizeF( _scrollViewFrame.Width * m_questionImagesList.Count, _scrollViewFrame.Height);
			svImagePager.AutoresizingMask = UIViewAutoresizing.All;
		}

		public void svImagePager_Scrolled (object sender, EventArgs e){
			int _scrollViewDisplayIndex = (int) Math.Floor ((svImagePager.ContentOffset.X +svImagePager.Frame.Width/2) /svImagePager.Frame.Width);
			if (_scrollViewDisplayIndex == m_currentImageDisplayIndex) {
				return;
			}
		}

		public UIView GetZoomedView (UIScrollView aZoomedScrollView){
			return aZoomedScrollView.Subviews[0];
		}

		public override void ViewWillAppear (bool animated)
		{
			base.ViewWillAppear (animated);
			SetDefaultZoomForAllScrollView ();
			CenterAllScrollViewImages ();

		}

		private void SetZoomForPreviousScrollView(){
			if (svPreviousImageZoomView != null) {
				float _widthScale = svPreviousImageZoomView.Frame.Width / svPreviousImageZoomView.ContentSize.Width;
				float _heightScale = svPreviousImageZoomView.Frame.Height / svPreviousImageZoomView.ContentSize.Height;
				float _minScale = Math.Min (_widthScale, _heightScale);
				svPreviousImageZoomView.MinimumZoomScale = _minScale;
				svPreviousImageZoomView.MaximumZoomScale = 1;
				svPreviousImageZoomView.ZoomScale = _minScale;
			}
		}

		private void SetZoomForCurrentScrollView(){
			if (svCurrentImageZoomView != null) {
				float _widthScale = svCurrentImageZoomView.Frame.Width / svCurrentImageZoomView.ContentSize.Width;
				float _heightScale = svCurrentImageZoomView.Frame.Height / svCurrentImageZoomView.ContentSize.Height;
				float _minScale = Math.Min (_widthScale, _heightScale);
				svCurrentImageZoomView.MinimumZoomScale = _minScale;
				svCurrentImageZoomView.MaximumZoomScale = 1;
				svCurrentImageZoomView.ZoomScale = _minScale;
			}
		}

		private void SetZoomForNextScrollView(){
			if (svNextImageZoomView != null) {
				float _widthScale = svNextImageZoomView.Frame.Width / svNextImageZoomView.ContentSize.Width;
				float _heightScale = svNextImageZoomView.Frame.Height / svNextImageZoomView.ContentSize.Height;
				float _minScale = Math.Min (_widthScale, _heightScale);
				svNextImageZoomView.MinimumZoomScale = _minScale;
				svNextImageZoomView.MaximumZoomScale = 1;
				svNextImageZoomView.ZoomScale = _minScale;
			}
		}

		private void SetDefaultZoomForAllScrollView(){
			SetZoomForCurrentScrollView ();
			SetZoomForPreviousScrollView ();
			SetZoomForNextScrollView ();
		}

		private void CenterPreviousScrollViewImages(){
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
		}

		private void CenterCurrentScrollViewImages(){
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
		}

		private void CenterNextScrollViewImages(){
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

		private void CenterAllScrollViewImages(){
			CenterCurrentScrollViewImages ();
			CenterPreviousScrollViewImages ();
			CenterNextScrollViewImages ();
		}

	}
}

