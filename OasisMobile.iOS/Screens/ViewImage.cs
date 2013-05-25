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
		private UIBarButtonItem navBackButton = null;
		private UIColor m_imageBackgroundColor = UIColor.FromRGB (100, 100, 100);

		static bool UserInterfaceIdiomIsPhone {
			get { return UIDevice.CurrentDevice.UserInterfaceIdiom == UIUserInterfaceIdiom.Phone; }
		}

		public ViewImage (int aImageToDisplayID)
			: base (UserInterfaceIdiomIsPhone ? "ViewImage_iPhone" : "ViewImage_iPad", null)
		{
			m_questionImagesList = BusinessModel.Image.GetImagesBySQL (string.Format (
								    "SELECT tblImageToReturn.* FROM tblImage AS tblImageToReturn INNER JOIN tblImage AS tblOriginalImage " +
								    "ON tblImageToReturn.fkQuestionID = tblOriginalImage.fkQuestionID " +
									"WHERE tblOriginalImage.pkImageID={0} ORDER BY Title",aImageToDisplayID));
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
			this.Title = "Figure " + m_questionImagesList [m_currentImageDisplayIndex].Title;
			svImagePager.BackgroundColor = m_imageBackgroundColor;
			var navItem = new UINavigationItem (this.Title);
			navBackButton = new UIBarButtonItem (UIBarButtonSystemItem.Rewind);
			navBackButton.Clicked += navBackButton_Clicked;
			navItem.LeftBarButtonItems = new UIBarButtonItem[] { navBackButton };
			navBar.SetItems (new UINavigationItem[]{navItem}, false);

			RectangleF _scrollViewFrame = svImagePager.Frame;
			svImagePager.ContentSize = new SizeF (_scrollViewFrame.Width * m_questionImagesList.Count, _scrollViewFrame.Height);
			svImagePager.AutoresizingMask = UIViewAutoresizing.All;

			DisplayCurrentImageInScrollView ();

			svImagePager.Scrolled += svImagePager_Scrolled;
			// Perform any additional setup after loading the view, typically from a nib.
		}

		public void DisplayCurrentImageInScrollView ()
		{
			foreach (UIView _subView in svImagePager.Subviews) {
				//Clear all previous subviews
				_subView.RemoveFromSuperview ();
			}

			//Load the current Image
			//-------------------------
			BusinessModel.Image _currentImageRecord = m_questionImagesList [m_currentImageDisplayIndex];
			UIImage _currentImageDataToDisplay = UIImage.FromFile (_currentImageRecord.FilePath);

			UIImageView _currentImageViewToDisplay = new UIImageView (_currentImageDataToDisplay);
			_currentImageViewToDisplay.Frame = new RectangleF (0, 0, _currentImageDataToDisplay.Size.Width, _currentImageDataToDisplay.Size.Height);
			_currentImageViewToDisplay.UserInteractionEnabled = true;
			AttachImageViewTapGestureRecognizer (_currentImageViewToDisplay);

			RectangleF _currentImageScrollViewFrame = svImagePager.Frame;
			_currentImageScrollViewFrame.X = _currentImageScrollViewFrame.Width * m_currentImageDisplayIndex;
			_currentImageScrollViewFrame.Y = 0;
			svCurrentImageZoomView = new UIScrollView (_currentImageScrollViewFrame);
			svCurrentImageZoomView.AutoresizingMask = UIViewAutoresizing.All;

			svCurrentImageZoomView.BackgroundColor = m_imageBackgroundColor;
			svCurrentImageZoomView.ContentSize = _currentImageDataToDisplay.Size;
			svCurrentImageZoomView.AddSubview (_currentImageViewToDisplay);
			svCurrentImageZoomView.ViewForZoomingInScrollView = GetZoomedView;
			svCurrentImageZoomView.DidZoom += ZoomView_DidZoom;
			svImagePager.AddSubview (svCurrentImageZoomView);

			if (m_currentImageDisplayIndex > 0) {
				//Load previous image
				BusinessModel.Image _previousImageRecord = m_questionImagesList [m_currentImageDisplayIndex-1];
				UIImage _previousImageDataToDisplay = UIImage.FromFile (_previousImageRecord.FilePath);

				UIImageView _previousImageViewToDisplay = new UIImageView (_previousImageDataToDisplay);
				_previousImageViewToDisplay.Frame = new RectangleF (0, 0, _previousImageDataToDisplay.Size.Width, _previousImageDataToDisplay.Size.Height);
				_previousImageViewToDisplay.UserInteractionEnabled = true;
				AttachImageViewTapGestureRecognizer (_previousImageViewToDisplay);

				RectangleF _previousImageScrollViewFrame = svImagePager.Frame;
				_previousImageScrollViewFrame.X = _previousImageScrollViewFrame.Width * (m_currentImageDisplayIndex - 1);
				_previousImageScrollViewFrame.Y = 0;
				svPreviousImageZoomView = new UIScrollView (_previousImageScrollViewFrame);
				svPreviousImageZoomView.AutoresizingMask = UIViewAutoresizing.All;

				svPreviousImageZoomView.BackgroundColor = m_imageBackgroundColor;
				svPreviousImageZoomView.ContentSize = _previousImageDataToDisplay.Size;
				svPreviousImageZoomView.AddSubview (_previousImageViewToDisplay);
				svPreviousImageZoomView.ViewForZoomingInScrollView = GetZoomedView;
				svPreviousImageZoomView.DidZoom += ZoomView_DidZoom;
				svImagePager.AddSubview (svPreviousImageZoomView);
			} else {
				svPreviousImageZoomView = null;
			}

			if (m_currentImageDisplayIndex < m_questionImagesList.Count - 1) {
				//Load next image
				BusinessModel.Image _nextImageRecord = m_questionImagesList [m_currentImageDisplayIndex+1];
				UIImage _nextImageDataToDisplay = UIImage.FromFile (_nextImageRecord.FilePath);

				UIImageView _nextImageViewToDisplay = new UIImageView (_nextImageDataToDisplay);
				_nextImageViewToDisplay.Frame = new RectangleF (0, 0, _nextImageDataToDisplay.Size.Width, _nextImageDataToDisplay.Size.Height);
				_nextImageViewToDisplay.UserInteractionEnabled = true;
				AttachImageViewTapGestureRecognizer (_nextImageViewToDisplay);

				RectangleF _nextImageScrollViewFrame = svImagePager.Frame;
				_nextImageScrollViewFrame.X = _nextImageScrollViewFrame.Width * (m_currentImageDisplayIndex + 1);
				_nextImageScrollViewFrame.Y = 0;
				svNextImageZoomView = new UIScrollView (_nextImageScrollViewFrame);
				svNextImageZoomView.AutoresizingMask = UIViewAutoresizing.All;

				svNextImageZoomView.BackgroundColor = m_imageBackgroundColor;
				svNextImageZoomView.ContentSize = _nextImageDataToDisplay.Size;
				svNextImageZoomView.AddSubview (_nextImageViewToDisplay);
				svNextImageZoomView.ViewForZoomingInScrollView = GetZoomedView;
				svNextImageZoomView.DidZoom += ZoomView_DidZoom;
				svImagePager.AddSubview (svNextImageZoomView);
			} else {
				svNextImageZoomView = null;
			}
		

			svImagePager.SetContentOffset (new PointF(svImagePager.Frame.Width * m_currentImageDisplayIndex,0),
			                               false);
		}

		public override void ViewDidLayoutSubviews ()
		{
			base.ViewDidLayoutSubviews ();

			RectangleF _scrollViewFrame = svImagePager.Frame;
			svImagePager.ContentSize = new SizeF (_scrollViewFrame.Width * m_questionImagesList.Count, _scrollViewFrame.Height);
			svImagePager.AutoresizingMask = UIViewAutoresizing.All;
			svImagePager.SetContentOffset (new PointF(svImagePager.Frame.Width * m_currentImageDisplayIndex,0),
			                               false);
			SetZoomForAllScrollView ();
			CenterAllScrollViewImages ();
		}

		public void svImagePager_Scrolled (object sender, EventArgs e)
		{
			int _scrollViewDisplayIndex = (int)Math.Floor ((svImagePager.ContentOffset.X +svImagePager.Frame.Width/2) /svImagePager.Frame.Width);
			if (_scrollViewDisplayIndex == m_currentImageDisplayIndex) {
				return;
			}

			if (_scrollViewDisplayIndex == m_currentImageDisplayIndex + 1) {
				//Set the next image as current one and load the next one
				//-----------------------------------------------------

				//Remove the previous image scrollview  from scrollview to save memory
				if (svPreviousImageZoomView != null) {
					svPreviousImageZoomView.RemoveFromSuperview ();
				}
				//Set the previous image scrollview to the current one and set the current image scroll view to use the next one
				svPreviousImageZoomView = svCurrentImageZoomView;
				svCurrentImageZoomView = svNextImageZoomView;

				m_currentImageDisplayIndex ++;

				if (m_currentImageDisplayIndex < m_questionImagesList.Count - 1) {
					//Load the next image to the scrollview
					BusinessModel.Image _nextImageRecord = m_questionImagesList [m_currentImageDisplayIndex+1];
					UIImage _nextImageDataToDisplay = UIImage.FromFile (_nextImageRecord.FilePath);

					UIImageView _nextImageViewToDisplay = new UIImageView (_nextImageDataToDisplay);
					_nextImageViewToDisplay.Frame = new RectangleF (0, 0, _nextImageDataToDisplay.Size.Width, _nextImageDataToDisplay.Size.Height);
					_nextImageViewToDisplay.UserInteractionEnabled = true;
					AttachImageViewTapGestureRecognizer (_nextImageViewToDisplay);

					RectangleF _nextImageScrollViewFrame = svImagePager.Frame;
					_nextImageScrollViewFrame.X = _nextImageScrollViewFrame.Width * (m_currentImageDisplayIndex + 1);
					_nextImageScrollViewFrame.Y = 0;
					svNextImageZoomView = new UIScrollView (_nextImageScrollViewFrame);
					svNextImageZoomView.AutoresizingMask = UIViewAutoresizing.All;

					svNextImageZoomView.BackgroundColor = m_imageBackgroundColor;
					svNextImageZoomView.ContentSize = _nextImageDataToDisplay.Size;
					svNextImageZoomView.AddSubview (_nextImageViewToDisplay);
					svNextImageZoomView.ViewForZoomingInScrollView = GetZoomedView;
					svNextImageZoomView.DidZoom += ZoomView_DidZoom;
					svImagePager.AddSubview (svNextImageZoomView);
				} else {
					//There is no next image available
					svNextImageZoomView = null;
				}

			} else if (_scrollViewDisplayIndex == m_currentImageDisplayIndex - 1) {
				//Set the previous image as current one and load the one before the previous image
				//-----------------------------------------------------

				//Remove the next image scrollview  from scrollview to save memory
				if (svNextImageZoomView != null) {
					svNextImageZoomView.RemoveFromSuperview ();
				}
				//Set the next image scrollview to the current one and set the current image scroll view to use the previous one
				svNextImageZoomView = svCurrentImageZoomView;
				svCurrentImageZoomView = svPreviousImageZoomView;

				m_currentImageDisplayIndex --;

				if (m_currentImageDisplayIndex > 0) {
					//Load previous image
					BusinessModel.Image _previousImageRecord = m_questionImagesList [m_currentImageDisplayIndex-1];
					UIImage _previousImageDataToDisplay = UIImage.FromFile (_previousImageRecord.FilePath);

					UIImageView _previousImageViewToDisplay = new UIImageView (_previousImageDataToDisplay);
					_previousImageViewToDisplay.Frame = new RectangleF (0, 0, _previousImageDataToDisplay.Size.Width, _previousImageDataToDisplay.Size.Height);
					_previousImageViewToDisplay.UserInteractionEnabled = true;
					AttachImageViewTapGestureRecognizer (_previousImageViewToDisplay);

					RectangleF _previousImageScrollViewFrame = svImagePager.Frame;
					_previousImageScrollViewFrame.X = _previousImageScrollViewFrame.Width * (m_currentImageDisplayIndex - 1);
					_previousImageScrollViewFrame.Y = 0;
					svPreviousImageZoomView = new UIScrollView (_previousImageScrollViewFrame);
					svPreviousImageZoomView.AutoresizingMask = UIViewAutoresizing.All;

					svPreviousImageZoomView.BackgroundColor = m_imageBackgroundColor;
					svPreviousImageZoomView.ContentSize = _previousImageDataToDisplay.Size;
					svPreviousImageZoomView.AddSubview (_previousImageViewToDisplay);
					svPreviousImageZoomView.ViewForZoomingInScrollView = GetZoomedView;
					svPreviousImageZoomView.DidZoom += ZoomView_DidZoom;
					svImagePager.AddSubview (svPreviousImageZoomView);
				} else {
					svPreviousImageZoomView = null;
				}
			} else {
				throw new Exception ("Scroll view index should only return the image before or after the current image");
			}

			this.Title = "Figure " + m_questionImagesList [m_currentImageDisplayIndex].Title;
			navBar.TopItem.Title = this.Title;
		}

		public UIView GetZoomedView (UIScrollView aZoomedScrollView)
		{
			return aZoomedScrollView.Subviews [0];
		}

		public void ZoomView_DidZoom (object sender, EventArgs e)
		{
			UIScrollView _zoomedScrollView = (UIScrollView)sender;

			SizeF _scrollViewBound = _zoomedScrollView.Bounds.Size;
			UIImageView _childImageView = (UIImageView)_zoomedScrollView.Subviews [0];
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

		public override void ViewWillAppear (bool animated)
		{
			base.ViewWillAppear (animated);
//			SetZoomForAllScrollView ();
//			CenterAllScrollViewImages ();
		}

		private void navBackButton_Clicked (object sender, EventArgs e)
		{
			this.DismissViewController (true, null);
		}

		private void AttachImageViewTapGestureRecognizer(UIImageView aImageViewToAttach){
			UITapGestureRecognizer _tapGesture = new UITapGestureRecognizer ();
			_tapGesture.AddTarget (() =>{
				HandleImageTapGesture (_tapGesture);
			});
			aImageViewToAttach.AddGestureRecognizer (_tapGesture);
		}

		private void HandleImageTapGesture (UITapGestureRecognizer aTapGestureObj)
		{
			if (aTapGestureObj.State == UIGestureRecognizerState.Recognized) {
				this.DismissViewController (true, null);
			}
		}

		private void SetZoomForPreviousScrollView ()
		{
			if (svPreviousImageZoomView != null) {
				float _minimumScaleBeforeUpdate = svPreviousImageZoomView.MinimumZoomScale;
				float _zoomScaleBeforeUpdate = svPreviousImageZoomView.ZoomScale;
				RectangleF _imageViewFrame = svPreviousImageZoomView.Subviews [0].Frame;
				float _widthScale = svPreviousImageZoomView.Frame.Width / _imageViewFrame.Width;
				float _heightScale = svPreviousImageZoomView.Frame.Height / _imageViewFrame.Height;
				float _minScale = Math.Min (_widthScale, _heightScale);
				if (_minScale > 1) {
					_minScale = 1;
				}
				svPreviousImageZoomView.MinimumZoomScale = _minScale;
				float _targetMaxZoomScale = _minScale * 8;
				if (_targetMaxZoomScale < 1) {
					//If the target maximum zoom is still not the regular image scale, we put the maxs to 1
					svPreviousImageZoomView.MaximumZoomScale = 1;
				} else if (_targetMaxZoomScale > 3) {
					//If the target zoom scale, is larger than 3, we will set it to 3 so that the image dont look too blurry
					svPreviousImageZoomView.MaximumZoomScale = 3;
				} else {
					//Otherwise, just use the target max zoom
					svPreviousImageZoomView.MaximumZoomScale = _targetMaxZoomScale;
				}

				if (_minimumScaleBeforeUpdate == _zoomScaleBeforeUpdate) {
					//This is needed so that if the image minimum scale gets smaller to display the full image, we still see the full image instead of the semi zoomed in one
					svPreviousImageZoomView.ZoomScale = _minScale;
				}

			
			}
		}

		private void SetZoomForCurrentScrollView ()
		{
			if (svCurrentImageZoomView != null) {
				float _minimumScaleBeforeUpdate = svCurrentImageZoomView.MinimumZoomScale;
				float _zoomScaleBeforeUpdate = svCurrentImageZoomView.ZoomScale;
				RectangleF _imageViewFrame = svCurrentImageZoomView.Subviews [0].Frame;
				float _widthScale = svCurrentImageZoomView.Frame.Width / _imageViewFrame.Width;
				float _heightScale = svCurrentImageZoomView.Frame.Height / _imageViewFrame.Height;
				float _minScale = Math.Min (_widthScale, _heightScale);
				if (_minScale > 1) {
					_minScale = 1;
				}
				svCurrentImageZoomView.MinimumZoomScale = _minScale;
				float _targetMaxZoomScale = _minScale * 8;
				if (_targetMaxZoomScale < 1) {
					//If the target maximum zoom is still not the regular image scale, we put the maxs to 1
					svCurrentImageZoomView.MaximumZoomScale = 1;
				} else if (_targetMaxZoomScale > 3) {
					//If the target zoom scale, is larger than 3, we will set it to 3 so that the image dont look too blurry
					svCurrentImageZoomView.MaximumZoomScale = 3;
				} else {
					//Otherwise, just use the target max zoom
					svCurrentImageZoomView.MaximumZoomScale = _targetMaxZoomScale;
				}

				if (_minimumScaleBeforeUpdate == _zoomScaleBeforeUpdate) {
					//This is needed so that if the image minimum scale gets smaller to display the full image, we still see the full image instead of the semi zoomed in one
					svCurrentImageZoomView.ZoomScale = _minScale;
				}
		
			
			}
		}

		private void SetZoomForNextScrollView ()
		{
			if (svNextImageZoomView != null) {
				float _minimumScaleBeforeUpdate = svNextImageZoomView.MinimumZoomScale;
				float _zoomScaleBeforeUpdate = svNextImageZoomView.ZoomScale;
				RectangleF _imageViewFrame = svNextImageZoomView.Subviews [0].Frame;
				float _widthScale = svNextImageZoomView.Frame.Width / _imageViewFrame.Width;
				float _heightScale = svNextImageZoomView.Frame.Height / _imageViewFrame.Height;
				float _minScale = Math.Min (_widthScale, _heightScale);
				if (_minScale > 1) {
					_minScale = 1;
				}
				svNextImageZoomView.MinimumZoomScale = _minScale;
				float _targetMaxZoomScale = _minScale * 8;
				if (_targetMaxZoomScale < 1) {
					//If the target maximum zoom is still not the regular image scale, we put the maxs to 1
					svNextImageZoomView.MaximumZoomScale = 1;
				} else if (_targetMaxZoomScale > 3) {
					//If the target zoom scale, is larger than 3, we will set it to 3 so that the image dont look too blurry
					svNextImageZoomView.MaximumZoomScale = 3;
				} else {
					//Otherwise, just use the target max zoom
					svNextImageZoomView.MaximumZoomScale = _targetMaxZoomScale;
				}
			
				if (_minimumScaleBeforeUpdate == _zoomScaleBeforeUpdate) {
					//This is needed so that if the image minimum scale gets smaller to display the full image, we still see the full image instead of the semi zoomed in one
					svNextImageZoomView.ZoomScale = _minScale;
				}
			
			}
		}

		private void SetZoomForAllScrollView ()
		{
			SetZoomForCurrentScrollView ();
			SetZoomForPreviousScrollView ();
			SetZoomForNextScrollView ();
		}

		private void CenterPreviousScrollViewImages ()
		{
			if (svPreviousImageZoomView != null) {
				SizeF _scrollViewBound = svPreviousImageZoomView.Bounds.Size;
				UIImageView _childImageView = (UIImageView)svPreviousImageZoomView.Subviews [0];
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

		private void CenterCurrentScrollViewImages ()
		{
			if (svCurrentImageZoomView != null) {
				SizeF _scrollViewBound = svCurrentImageZoomView.Bounds.Size;
				UIImageView _childImageView = (UIImageView)svCurrentImageZoomView.Subviews [0];
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

		private void CenterNextScrollViewImages ()
		{
			if (svNextImageZoomView != null) {
				SizeF _scrollViewBound = svNextImageZoomView.Bounds.Size;
				UIImageView _childImageView = (UIImageView)svNextImageZoomView.Subviews [0];
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

		private void CenterAllScrollViewImages ()
		{
			CenterCurrentScrollViewImages ();
			CenterPreviousScrollViewImages ();
			CenterNextScrollViewImages ();
		}
	}
}

