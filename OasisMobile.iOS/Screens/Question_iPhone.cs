using System;
using System.Drawing;
using MonoTouch.Foundation;
using MonoTouch.UIKit;
using System.Collections.Generic;
using System.Linq;

namespace OasisMobile.iOS
{
	public partial class Question_iPhone : UIViewController
	{
		private BusinessModel.UserQuestion m_currentQuestionToDisplay;
		private int m_currentQuestionToDisplayIndex = 0;
		private int m_totalQuestionInExam = 0;
		private UITableView tblvCurrentQuestion = null;
		private UITableView tblvPreviousQuestion = null;
		private UITableView tblvNextQuestion = null;

		public int CurrentQuestionToDisplayIndex{
			get{
				return m_currentQuestionToDisplayIndex;
			}
		}

		public int TotalQuestionInExam{
			get{
				return m_totalQuestionInExam;
			}
		}

		public BusinessModel.UserQuestion CurrentQuestionToDisplay{
			get{
				return m_currentQuestionToDisplay;
			}
		}


		public Question_iPhone (BusinessModel.UserQuestion aUserQuestion) : base ("Question_iPhone", null)
		{
			m_currentQuestionToDisplay = 
				(from x in AppSession.SelectedExamUserQuestionList 
				 where x.UserQuestionID == aUserQuestion.UserQuestionID select x).FirstOrDefault ();
			m_currentQuestionToDisplayIndex = AppSession.SelectedExamUserQuestionList.IndexOf (m_currentQuestionToDisplay);
			m_totalQuestionInExam = AppSession.SelectedExamUserQuestionList.Count;
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

			// Perform any additional setup after loading the view, typically from a nib.
			this.Title = string.Format ("{0} of {1}", m_currentQuestionToDisplay.Sequence,
			                            m_totalQuestionInExam);

			svQuestionPager.Scrolled += svQuestionPager_Scrolled;
		}

		public override void ViewDidLayoutSubviews ()
		{
			base.ViewDidLayoutSubviews ();

			//Make the scrollview content size to fit all question in exam
			RectangleF _scrollViewFrame = this.View.Frame;
			_scrollViewFrame.Width = _scrollViewFrame.Width * m_totalQuestionInExam;
			svQuestionPager.ContentSize = _scrollViewFrame.Size;

			// Create TableView for current question
			//----------------------------------------
			RectangleF _currentQuestionFrame = svQuestionPager.Frame;
			PointF _currentQuestionLocation = new PointF ();
			_currentQuestionLocation.X = svQuestionPager.Frame.Width * m_currentQuestionToDisplayIndex;
			_currentQuestionFrame.Location = _currentQuestionLocation;

			tblvCurrentQuestion = new UITableView (_currentQuestionFrame, UITableViewStyle.Grouped);
			tblvCurrentQuestion.Source =  new Question_iPhoneTableSource (m_currentQuestionToDisplay, this);
			svQuestionPager.AddSubview (tblvCurrentQuestion);

			if (m_currentQuestionToDisplayIndex > 0) {
				//We should have previous question here

				// Create TableView for previous question
				//----------------------------------------
				RectangleF _previousQuestionFrame = svQuestionPager.Frame;
				PointF _previousQuestionLocation = new PointF ();
				_previousQuestionLocation.X = svQuestionPager.Frame.Width * (m_currentQuestionToDisplayIndex-1);
				_previousQuestionFrame.Location = _previousQuestionLocation;

				tblvPreviousQuestion = new UITableView (_previousQuestionFrame, UITableViewStyle.Grouped);
				tblvPreviousQuestion.Source =  new Question_iPhoneTableSource (AppSession.SelectedExamUserQuestionList[m_currentQuestionToDisplayIndex-1], this);
				svQuestionPager.AddSubview (tblvPreviousQuestion);
			}
			if(m_currentQuestionToDisplayIndex < m_totalQuestionInExam - 1){
				//We should have next question here

				// Create TableView for next question
				//----------------------------------------
				RectangleF _nextQuestionFrame = svQuestionPager.Frame;
				PointF _nextQuestionLocation = new PointF ();
				_nextQuestionLocation.X = svQuestionPager.Frame.Width * (m_currentQuestionToDisplayIndex+1);
				_nextQuestionFrame.Location = _nextQuestionLocation;

				tblvNextQuestion = new UITableView (_nextQuestionFrame, UITableViewStyle.Grouped);
				tblvNextQuestion.Source =  new Question_iPhoneTableSource (AppSession.SelectedExamUserQuestionList[m_currentQuestionToDisplayIndex+1], this);
				svQuestionPager.AddSubview (tblvNextQuestion);
			}

			//Set the current scroll position
			svQuestionPager.SetContentOffset (_currentQuestionLocation,false);

		}

		private void svQuestionPager_Scrolled(object sender, EventArgs e){
			int _scrollViewDisplayIndex = (int) Math.Floor ((svQuestionPager.ContentOffset.X +svQuestionPager.Frame.Width/2) /svQuestionPager.Frame.Width);
			if (_scrollViewDisplayIndex == m_currentQuestionToDisplayIndex) {
				return;
			}

			if (_scrollViewDisplayIndex == m_currentQuestionToDisplayIndex + 1) {
				//Set the next question as current one and load the next one
				//-----------------------------------------------------

				//Remove the previous question tableview from scrollview to save memory
				if(tblvPreviousQuestion!=null){
					tblvPreviousQuestion.RemoveFromSuperview ();
				}

				tblvPreviousQuestion = tblvCurrentQuestion; //The current question becomes the previous one
				tblvCurrentQuestion = tblvNextQuestion; //The next question becomes the current one

				m_currentQuestionToDisplayIndex ++;
				m_currentQuestionToDisplay = AppSession.SelectedExamUserQuestionList [m_currentQuestionToDisplayIndex];

				if (m_currentQuestionToDisplayIndex < m_totalQuestionInExam - 1) {
					//Load the question to populate to the next question tableview
					RectangleF _nextQuestionFrame = svQuestionPager.Frame;
					PointF _nextQuestionLocation = new PointF ();
					_nextQuestionLocation.X = svQuestionPager.Frame.Width * (m_currentQuestionToDisplayIndex+1);
					_nextQuestionFrame.Location = _nextQuestionLocation;
					tblvNextQuestion = new UITableView (_nextQuestionFrame, UITableViewStyle.Grouped);
					tblvNextQuestion.Source =  new Question_iPhoneTableSource (AppSession.SelectedExamUserQuestionList[m_currentQuestionToDisplayIndex+1], this);
					svQuestionPager.AddSubview (tblvNextQuestion);
				}else{
					//We are at the last question in exam, there are no next question available
					tblvNextQuestion = null;
				}

			} else if (_scrollViewDisplayIndex == m_currentQuestionToDisplayIndex - 1) {
				//Set the previous question as current one and load the previous one
				//-----------------------------------------------------

				//Remove the next question tableview from scrollview to save memory
				if(tblvNextQuestion!=null){
					tblvNextQuestion.RemoveFromSuperview ();
				}

				tblvNextQuestion = tblvCurrentQuestion; //The current question becomes the next question
				tblvCurrentQuestion = tblvPreviousQuestion; //The previous question becomes the current one

				m_currentQuestionToDisplayIndex --;
				m_currentQuestionToDisplay = AppSession.SelectedExamUserQuestionList [m_currentQuestionToDisplayIndex];

				if (m_currentQuestionToDisplayIndex > 0) {
					//Load the question to populate to the previous question tableview
					RectangleF _previousQuestionFrame = svQuestionPager.Frame;
					PointF _previousQuestionLocation = new PointF ();
					_previousQuestionLocation.X = svQuestionPager.Frame.Width * (m_currentQuestionToDisplayIndex-1);
					_previousQuestionFrame.Location = _previousQuestionLocation;
					tblvPreviousQuestion = new UITableView (_previousQuestionFrame, UITableViewStyle.Grouped);
					tblvPreviousQuestion.Source =  new Question_iPhoneTableSource (AppSession.SelectedExamUserQuestionList[m_currentQuestionToDisplayIndex-1], this);
					svQuestionPager.AddSubview (tblvPreviousQuestion);
				}else{
					//We are at the first question in exam, there are no previous question available
					tblvPreviousQuestion = null;
				}

			} else {
				throw new Exception ("Scroll view index should only return the question before or after the current question");
			}

			this.Title = string.Format ("{0} of {1}", m_currentQuestionToDisplay.Sequence,
			                            m_totalQuestionInExam);

		}

		private void GoToNextQuestion(){
			PointF _targetLocation = new PointF (svQuestionPager.Frame.Width * (m_currentQuestionToDisplayIndex + 1), 0);
			svQuestionPager.SetContentOffset (_targetLocation,true);
		
		}

		private void GoToPreviousQuestion(){
			PointF _targetLocation = new PointF (svQuestionPager.Frame.Width * (m_currentQuestionToDisplayIndex - 1), 0);
			svQuestionPager.SetContentOffset (_targetLocation,true);
		}

		public void ReloadCurrentQuestion(){
			m_currentQuestionToDisplay = AppSession.SelectedExamUserQuestionList [m_currentQuestionToDisplayIndex];
			tblvCurrentQuestion.Source = new Question_iPhoneTableSource (AppSession.SelectedExamUserQuestionList[m_currentQuestionToDisplayIndex], this);
			tblvCurrentQuestion.ReloadData ();
		}



		public class Question_iPhoneTableSource : UITableViewSource
		{
			public enum SubmittedQuestionViewSections
			{
				QuestionStemAndImages = 0
,
				QuestionAnswerOptions = 1
,
				QuestionCommentary = 2
,
				QuestionReferences = 3
			}

			public enum UnsubmittedQuestionViewSections
			{
				QuestionStemAndImages = 0
,
				QuestionAnswerOptions = 1
,
				SubmitAnswerButton = 2
			}

			private Question_iPhone m_currentViewController;
			private BusinessModel.UserQuestion m_userQuestion;
			private BusinessModel.Question m_question;
			private List<BusinessModel.Image> m_questionImages;
			private List<BusinessModel.UserAnswerOptionDetail> m_questionAnswerOptions;
			private bool m_showQuestionAnswer;
			private UIButton btnSubmitQuestionAnswer;

			public Question_iPhoneTableSource (BusinessModel.UserQuestion aUserQuestion, Question_iPhone aParentViewControlller)
			{
				m_currentViewController = aParentViewControlller;
				m_userQuestion = aUserQuestion;
				m_question = BusinessModel.Question.GetQuestionByQuestionID (aUserQuestion.QuestionID);
				m_questionImages = BusinessModel.Image.GetImagesByQuestionID (aUserQuestion.QuestionID);
				if (m_questionImages == null) {
					m_questionImages = new List<BusinessModel.Image> ();
				}
				m_questionImages = (from x in m_questionImages orderby x.Title select x).ToList ();
				m_questionAnswerOptions = BusinessModel.UserAnswerOptionDetail.GetUserAnswerOptionDetailListByUserQuestionID (aUserQuestion.UserQuestionID);
				m_questionAnswerOptions = (from x in m_questionAnswerOptions orderby x.Sequence select x).ToList ();

				if (AppSession.SelectedUserExam.IsSubmitted || (aUserQuestion.HasAnswered && AppSession.SelectedUserExam.IsLearningMode)) {
					m_showQuestionAnswer = true;
				} else {
					m_showQuestionAnswer = false;
				}

			}
			#region implemented abstract members of UITableViewSource
			
			public override int RowsInSection (UITableView tableview, int section)
			{
				if (m_showQuestionAnswer) {
					switch (section) {
					case (int)SubmittedQuestionViewSections.QuestionStemAndImages:
						return m_questionImages.Count + 2;
					case (int)SubmittedQuestionViewSections.QuestionAnswerOptions:
						return m_questionAnswerOptions.Count;
					case (int)SubmittedQuestionViewSections.QuestionCommentary:
						return 1;
					case (int)SubmittedQuestionViewSections.QuestionReferences:
						return 1;
					default:
						return 0;
					}
				} else {
					switch (section) {
					case (int)UnsubmittedQuestionViewSections .QuestionStemAndImages:
						return m_questionImages.Count + 2; //Question images + Question Stem + Question Leadin
					case (int)UnsubmittedQuestionViewSections.QuestionAnswerOptions:
						return m_questionAnswerOptions.Count;
					case (int)UnsubmittedQuestionViewSections.SubmitAnswerButton:
						return 1;
					default:
						return 0;
					}
				}
			
				
			}



			public override UITableViewCell GetCell (UITableView tableView, NSIndexPath indexPath)
			{
				UITableViewCell cell;

				if (m_showQuestionAnswer) {
					switch (indexPath.Section) {
					case (int)SubmittedQuestionViewSections.QuestionStemAndImages:
						//cell.BackgroundView = new UIView (RectangleF.Empty); // The question stem/images will not have separators
						//cell.Layer.BorderWidth=0;
						if (indexPath.Row == 0) {
							cell = tableView.DequeueReusableCell ("cell");
							if (cell == null) {
								cell = new UITableViewCell (UITableViewCellStyle.Default, "cell");
							}
							cell.TextLabel.Font = UIFont.SystemFontOfSize (14);
							cell.TextLabel.Text = m_question.Stem;
							cell.TextLabel.Lines = 0;
							cell.TextLabel.LineBreakMode = UILineBreakMode.WordWrap;
						} else if (indexPath.Row == m_questionImages.Count + 1) {
							cell = tableView.DequeueReusableCell ("cell");
							if (cell == null) {
								cell = new UITableViewCell (UITableViewCellStyle.Default, "cell");
							}
							cell.TextLabel.Font = UIFont.SystemFontOfSize (14);
							cell.TextLabel.Text = m_question.LeadIn;
							cell.TextLabel.Lines = 0;
							cell.TextLabel.LineBreakMode = UILineBreakMode.WordWrap;
						} else {
		
							cell = tableView.DequeueReusableCell ("imageCell");
							if (cell == null) {
								cell = new CustomImageCell ("imageCell");
							}
							UIImage _imageAtRow = UIImage.FromFile (m_questionImages [indexPath.Row - 1].FilePath);
							cell.ImageView.Image = _imageAtRow;

							cell.ImageView.GestureRecognizers = new UIGestureRecognizer[] { };
							cell.ImageView.UserInteractionEnabled = true;
							UITapGestureRecognizer _tapGesture = new UITapGestureRecognizer ();
							_tapGesture.AddTarget (() =>{
								HandleImageTapGesture(_tapGesture, m_questionImages [indexPath.Row - 1].ImageID);
							});
							cell.ImageView.AddGestureRecognizer (_tapGesture);
						}
						break;
					case (int)SubmittedQuestionViewSections.QuestionAnswerOptions:
						cell = tableView.DequeueReusableCell ("answerOptionCell");
						if (cell == null) {
							cell = new UITableViewCell (UITableViewCellStyle.Default, "answerOptionCell");
						}
						BusinessModel.UserAnswerOptionDetail _answerOption = m_questionAnswerOptions [indexPath.Row];
						cell.TextLabel.Font = UIFont.SystemFontOfSize (14);
						cell.TextLabel.Text = (Char)(Convert.ToInt32 ('A') + _answerOption.Sequence) + ". " + _answerOption.AnswerOptionText;
						cell.TextLabel.Lines = 0;
						cell.TextLabel.LineBreakMode = UILineBreakMode.WordWrap;
						if (_answerOption.IsCorrect) {
							cell.BackgroundColor = UIColor.Green;
						} else {
							if (_answerOption.IsSelected) {
								cell.BackgroundColor = UIColor.Red;
							} else {
								cell.BackgroundColor = UIColor.White;
							}
						}
						break;
					case (int)SubmittedQuestionViewSections.QuestionCommentary:
						cell = tableView.DequeueReusableCell ("cell");
						if (cell == null) {
							cell = new UITableViewCell (UITableViewCellStyle.Default, "cell");
						}
						cell.TextLabel.Font = UIFont.SystemFontOfSize (14);
						cell.TextLabel.Text = m_question.Commentary;
						cell.TextLabel.Lines = 0;
						cell.TextLabel.LineBreakMode = UILineBreakMode.WordWrap;
						break;
					case (int)SubmittedQuestionViewSections.QuestionReferences:
						cell = tableView.DequeueReusableCell ("cell");
						if (cell == null) {
							cell = new UITableViewCell (UITableViewCellStyle.Default, "cell");
						}
						cell.TextLabel.Font = UIFont.SystemFontOfSize (14);
						cell.TextLabel.Text = m_question.Reference;
						cell.TextLabel.Lines = 0;
						cell.TextLabel.LineBreakMode = UILineBreakMode.WordWrap;
						break;
					default:
						cell = tableView.DequeueReusableCell ("cell");
						if (cell == null) {
							cell = new UITableViewCell (UITableViewCellStyle.Default, "cell");
						}
						break;
					}
				} else {
					switch (indexPath.Section) {
					case (int)UnsubmittedQuestionViewSections.QuestionStemAndImages:
						//cell.BackgroundView = new UIView (RectangleF.Empty); // The question stem/images will not have separators
						//cell.Layer.BorderWidth=0;
						if (indexPath.Row == 0) {
							cell = tableView.DequeueReusableCell ("cell");
							if (cell == null) {
								cell = new UITableViewCell (UITableViewCellStyle.Default, "cell");
							}
							cell.TextLabel.Font = UIFont.SystemFontOfSize (14);
							cell.TextLabel.Text = m_question.Stem.Replace ("<br />","\n");
							cell.TextLabel.Lines = 0;
							cell.TextLabel.LineBreakMode = UILineBreakMode.WordWrap;
						} else if (indexPath.Row == m_questionImages.Count + 1) {
							cell = tableView.DequeueReusableCell ("cell");
							if (cell == null) {
								cell = new UITableViewCell (UITableViewCellStyle.Default, "cell");
							}
							cell.TextLabel.Font = UIFont.SystemFontOfSize (14);
							cell.TextLabel.Text = m_question.LeadIn;
							cell.TextLabel.Lines = 0;
							cell.TextLabel.LineBreakMode = UILineBreakMode.WordWrap;
						} else {
							cell = tableView.DequeueReusableCell ("imageCell");
							if (cell == null) {
								cell = new CustomImageCell ("imageCell");
							}
							UIImage _imageAtRow = UIImage.FromFile (m_questionImages [indexPath.Row - 1].FilePath);
							cell.ImageView.Image = _imageAtRow;

							cell.ImageView.GestureRecognizers = new UIGestureRecognizer[] { };
							cell.ImageView.UserInteractionEnabled = true;
							UITapGestureRecognizer _tapGesture = new UITapGestureRecognizer ();
							_tapGesture.AddTarget (() =>{
								HandleImageTapGesture(_tapGesture, m_questionImages [indexPath.Row - 1].ImageID);
							});
							cell.ImageView.AddGestureRecognizer (_tapGesture);
						}
						break;
					case (int)UnsubmittedQuestionViewSections.QuestionAnswerOptions:
						cell = tableView.DequeueReusableCell ("answerOptionCell");
						if (cell == null) {
							cell = new UITableViewCell (UITableViewCellStyle.Default, "answerOptionCell");
						}
						BusinessModel.UserAnswerOptionDetail _answerOption = m_questionAnswerOptions [indexPath.Row];
						cell.TextLabel.Font = UIFont.SystemFontOfSize (14);
						cell.TextLabel.Text = (Char)(Convert.ToInt32 ('A') + _answerOption.Sequence) + ". " + _answerOption.AnswerOptionText;
						cell.TextLabel.Lines = 0;
						cell.TextLabel.LineBreakMode = UILineBreakMode.WordWrap;
						break;
					case (int)UnsubmittedQuestionViewSections.SubmitAnswerButton:
						cell = tableView.DequeueReusableCell ("buttonCell");
						if (cell == null) {
							cell = new UITableViewCell (UITableViewCellStyle.Default, "buttonCell");
						}

						btnSubmitQuestionAnswer = new UIButton (UIButtonType.RoundedRect);
						btnSubmitQuestionAnswer.AutoresizingMask = UIViewAutoresizing.FlexibleWidth;
						btnSubmitQuestionAnswer.Frame = new System.Drawing.RectangleF (0, 0, cell.ContentView.Frame.Width, 36);
						btnSubmitQuestionAnswer.SetTitle ("Submit", UIControlState.Normal);
						btnSubmitQuestionAnswer.TouchUpInside += btnSubmitQuestionAnswer_Click;

						foreach (UIView _subview in cell.ContentView.Subviews) {
							_subview.RemoveFromSuperview ();
						}
						cell.ContentView.AddSubview (btnSubmitQuestionAnswer);
						break;
					default:
						cell = tableView.DequeueReusableCell ("cell");
						if (cell == null) {
							cell = new UITableViewCell (UITableViewCellStyle.Default, "cell");
						}
						break;
					}
				}
				return cell;
			}
			#endregion
			
			public override int NumberOfSections (UITableView tableView)
			{
				if (m_showQuestionAnswer) {
					return Enum.GetValues (typeof(SubmittedQuestionViewSections)).Length;
				} else {
					return Enum.GetValues (typeof(UnsubmittedQuestionViewSections)).Length;
				}
				// TODO: Implement - see: http://go-mono.com/docs/index.aspx?link=T%3aMonoTouch.Foundation.ModelAttribute
			}

			public override NSIndexPath WillSelectRow (UITableView tableView, NSIndexPath indexPath)
			{
				// NOTE: Don't call the base implementation on a Model class
				// see http://docs.xamarin.com/ios/tutorials/Events%2c_Protocols_and_Delegates 
				if (m_showQuestionAnswer) {
					return null;
				} else {
					if (indexPath.Section == (int)UnsubmittedQuestionViewSections.QuestionAnswerOptions) {
						return indexPath;
					} else {
						return null;
					}
				}
			}

			public override void RowSelected (UITableView tableView, NSIndexPath indexPath)
			{
				// NOTE: Don't call the base implementation on a Model class
				// see http://docs.xamarin.com/ios/tutorials/Events%2c_Protocols_and_Delegates 
				if (m_showQuestionAnswer) {
					//We dont allow any selection when we already display the answer
					tableView.DeselectRow (indexPath, false);
				} else {
					if (indexPath.Section == (int)UnsubmittedQuestionViewSections.QuestionAnswerOptions) {
						//Update the source global variable so we can pull the answer data later on
						BusinessModel.UserAnswerOptionDetail _selectedAnswerOptionObj = null;
						for (int i=0; i<m_questionAnswerOptions.Count; i++) {
							BusinessModel.UserAnswerOptionDetail _answerOptionObj = m_questionAnswerOptions [i];
							if (i == indexPath.Row) {
								_answerOptionObj.IsSelected = true;
								_selectedAnswerOptionObj = _answerOptionObj;
							} else {
								UITableViewCell _cellAtRow = tableView.CellAt (NSIndexPath.FromRowSection (i,(int)UnsubmittedQuestionViewSections.QuestionAnswerOptions));
								if(_cellAtRow != null){
									_cellAtRow.Selected = false;
								}
								_answerOptionObj.IsSelected = false;
							}
						}

						if (AppSession.SelectedUserExam.IsLearningMode && AppSettings.AutoSubmitResponse) {
							SubmitUserAnswer (_selectedAnswerOptionObj);
							//For learning mode, we update the interface to show the answer
							m_currentViewController.ReloadCurrentQuestion ();
						}else if(!AppSession.SelectedUserExam.IsLearningMode && AppSettings.AutoAdvanceQuestion){
							SubmitUserAnswer (_selectedAnswerOptionObj);
							//For examination mode, we go to the next answer
							if (m_currentViewController.CurrentQuestionToDisplayIndex < m_currentViewController.TotalQuestionInExam - 1) {
								m_currentViewController.GoToNextQuestion ();
							} else {
								m_currentViewController.NavigationController.PopViewControllerAnimated (true);
							}
						}

					} else {
						tableView.DeselectRow (indexPath, false);
					}
				}
			}

			public override float GetHeightForRow (UITableView tableView, NSIndexPath indexPath)
			{
				// NOTE: Don't call the base implementation on a Model class
				// see http://docs.xamarin.com/ios/tutorials/Events%2c_Protocols_and_Delegates 

				SizeF _bounds = new SizeF (tableView.Bounds.Width - 40, float.MaxValue);
				if (m_showQuestionAnswer) {
					switch (indexPath.Section) {
					case (int) SubmittedQuestionViewSections.QuestionStemAndImages:
						if (indexPath.Row == 0) {
							return tableView.StringSize (m_question.Stem, UIFont.SystemFontOfSize (14), _bounds, UILineBreakMode.WordWrap).Height + 20; // add 20 px as padding
						} else if (indexPath.Row == m_questionImages.Count + 1) {
							return tableView.StringSize (m_question.LeadIn, UIFont.SystemFontOfSize (14), _bounds, UILineBreakMode.WordWrap).Height + 20; // add 20 px as padding
						} else {
							UIImage _imageAtRow = UIImage.FromFile (m_questionImages [indexPath.Row - 1].FilePath);
							float _widthToHeightRatio = _imageAtRow.Size.Width / _imageAtRow.Size.Height;
							float _maxDimension = _bounds.Width;
							if (_widthToHeightRatio >= 1) {
								return _maxDimension / _widthToHeightRatio + 20;
							} else {
								return _maxDimension + 20;
							} 
						}
					case (int) SubmittedQuestionViewSections.QuestionAnswerOptions:
						return tableView.StringSize (m_questionAnswerOptions [indexPath.Row].AnswerOptionText, UIFont.SystemFontOfSize (14), _bounds, UILineBreakMode.WordWrap).Height + 20; // add 20 px as padding
					case (int) SubmittedQuestionViewSections.QuestionCommentary:
						return tableView.StringSize (m_question.Commentary, UIFont.SystemFontOfSize (14), _bounds, UILineBreakMode.WordWrap).Height + 20; 
					case (int) SubmittedQuestionViewSections.QuestionReferences:
						return tableView.StringSize (m_question.Reference, UIFont.SystemFontOfSize (14), _bounds, UILineBreakMode.WordWrap).Height + 20; 
					}
				} else {
					switch (indexPath.Section) {
					case (int) UnsubmittedQuestionViewSections.QuestionStemAndImages:
						if (indexPath.Row == 0) {
							return tableView.StringSize (m_question.Stem.Replace ("<br />","\n"), UIFont.SystemFontOfSize (14), _bounds, UILineBreakMode.WordWrap).Height + 20; // add 20 px as padding
						} else if (indexPath.Row == m_questionImages.Count + 1) {
							return tableView.StringSize (m_question.LeadIn, UIFont.SystemFontOfSize (14), _bounds, UILineBreakMode.WordWrap).Height + 20; // add 20 px as padding
						} else {
							UIImage _imageAtRow = UIImage.FromFile (m_questionImages [indexPath.Row - 1].FilePath);
							float _widthToHeightRatio = _imageAtRow.Size.Width / _imageAtRow.Size.Height;
							float _maxDimension = _bounds.Width;
							if (_widthToHeightRatio >= 1) {
								return _maxDimension / _widthToHeightRatio + 20;
							} else {
								return _maxDimension + 20;
							} 

						}
					case (int) UnsubmittedQuestionViewSections.QuestionAnswerOptions:
						return tableView.StringSize (m_questionAnswerOptions [indexPath.Row].AnswerOptionText, UIFont.SystemFontOfSize (14), _bounds, UILineBreakMode.WordWrap).Height + 20; // add 20 px as padding
					case (int) UnsubmittedQuestionViewSections.SubmitAnswerButton:
						return 44;
					}
				}

				return 0; //Catch all if we have not returned yet, should never reach here
	
			}

			public override string TitleForHeader (UITableView tableView, int section)
			{
				// NOTE: Don't call the base implementation on a Model class
				// see http://docs.xamarin.com/ios/tutorials/Events%2c_Protocols_and_Delegates 
				string _headerText = "";
				if (m_showQuestionAnswer) {
					switch (section) {
					case (int)SubmittedQuestionViewSections.QuestionStemAndImages:
						_headerText = "Question";
						break;
					case (int)SubmittedQuestionViewSections.QuestionAnswerOptions:
						_headerText = "Response";
						break;
					case (int)SubmittedQuestionViewSections.QuestionCommentary:
						_headerText = "Commentary";
						break;
					case (int)SubmittedQuestionViewSections.QuestionReferences:
						_headerText = "Reference";
						break;
					}
				} else {
					switch (section) {
					case (int)UnsubmittedQuestionViewSections.QuestionStemAndImages:
						_headerText = "Question";
						break;
					case (int)UnsubmittedQuestionViewSections.QuestionAnswerOptions:
						_headerText = "Response";
						break;
					case (int)UnsubmittedQuestionViewSections.SubmitAnswerButton:
						_headerText = "";
						break;
					}
				}

				return _headerText;
			}

			public override void WillDisplay (UITableView tableView, UITableViewCell cell, NSIndexPath indexPath)
			{
				// NOTE: Don't call the base implementation on a Model class
				// see http://docs.xamarin.com/ios/tutorials/Events%2c_Protocols_and_Delegates 
				if (m_showQuestionAnswer) {	
					cell.SelectionStyle = UITableViewCellSelectionStyle.None;
				} else {
					if (indexPath.Section == (int)UnsubmittedQuestionViewSections.SubmitAnswerButton) {
						cell.BackgroundView = null;
						cell.SelectionStyle = UITableViewCellSelectionStyle.None;
					} else if (indexPath.Section == (int)UnsubmittedQuestionViewSections.QuestionAnswerOptions) {
						cell.SelectionStyle = UITableViewCellSelectionStyle.Blue;
						//also select the cell if the cell contains the selected answer option. 
						//This is because selecting at getcell will show black bars
						if (m_questionAnswerOptions [indexPath.Row].IsSelected) {
						cell.Selected = true;
						}
						else{
							cell.Selected = false;
						}
					} else {
						cell.SelectionStyle = UITableViewCellSelectionStyle.None;
					}
				}
				
			}

			public void HandleImageTapGesture(UITapGestureRecognizer aRecognizer, int aImageID){
				if(aRecognizer.State == UIGestureRecognizerState.Recognized){
					ViewImage _viewImageController = new ViewImage (aImageID);
					m_currentViewController.PresentViewController (_viewImageController, true, null);
				}
			}

			private void btnSubmitQuestionAnswer_Click (object sender, EventArgs e)
			{
			 	BusinessModel.UserAnswerOption _previousSelectedAnswer =  BusinessModel.UserAnswerOption.GetFirstUserAnswerOptionBySQL (string.Format (
					"SELECT * FROM tblUserAnswerOption WHERE fkUserQuestionID={0} AND IsSelected=1",m_userQuestion.UserQuestionID));
				BusinessModel.UserAnswerOptionDetail _selectedAnswerOption = 
					(from x in m_questionAnswerOptions where x.IsSelected select x).FirstOrDefault ();
				if (_selectedAnswerOption == null) {
					UIAlertView _requiredFieldAlert = new UIAlertView ("Selection Required", "Please select an answer before continuing", null, "Ok", null);
					_requiredFieldAlert.Show ();
					return;
				}

			
				if (_previousSelectedAnswer == null || _selectedAnswerOption.UserAnswerOptionID == _previousSelectedAnswer.UserAnswerOptionID) {
					SubmitUserAnswer (_selectedAnswerOption);

					if (AppSession.SelectedUserExam.IsLearningMode) {
						//For learning mode, we update the interface to show the answer
						m_currentViewController.ReloadCurrentQuestion ();
					} else {
						//For examination mode, we go to the next answer
						if (m_currentViewController.CurrentQuestionToDisplayIndex < m_currentViewController.TotalQuestionInExam - 1) {
							m_currentViewController.GoToNextQuestion ();
						} else {
							m_currentViewController.NavigationController.PopViewControllerAnimated (true);
						}
					}

				} else {
					//The submit button should only shown in learning mode if the user has not answered, so we only need to code the examination mode to advance to next question
					//For examination mode, we go to the next answer
					if (AppSession.SelectedUserExam.IsLearningMode) {
						throw new Exception ("There should not be any submit button in learning mode when question has been answered");
					} else {
						if (m_currentViewController.CurrentQuestionToDisplayIndex < m_currentViewController.TotalQuestionInExam - 1) {
							m_currentViewController.GoToNextQuestion ();
						} else {
							m_currentViewController.NavigationController.PopViewControllerAnimated (true);
						}
					}

				}

		
			}

			private void SubmitUserAnswer(BusinessModel.UserAnswerOptionDetail aSelectedAnswerOption){
				int _hasAnsweredCorrectly;
				if (aSelectedAnswerOption.IsCorrect) {
					_hasAnsweredCorrectly = 1;
				} else {
					_hasAnsweredCorrectly = 0;
				}

				List<string> _queriesToExecute = new List<string> ();

				_queriesToExecute.Add (string.Format (
					"UPDATE tblUserAnswerOption SET IsSelected=1 WHERE pkUserAnswerOptionID={0}", 
					aSelectedAnswerOption.UserAnswerOptionID));
				_queriesToExecute.Add (string.Format (
					"UPDATE tblUserAnswerOption SET IsSelected=0 " +
					"WHERE pkUserAnswerOptionID!={0} AND fkUserQuestionID={1}", 
					aSelectedAnswerOption.UserAnswerOptionID, aSelectedAnswerOption.UserQuestionID));
				_queriesToExecute.Add (string.Format (
					"UPDATE tblUserQuestion SET HasAnswered=1, HasAnsweredCorrectly={0}, AnsweredDateTime='{1}', DoSync=1 " +
					"WHERE pkUserQuestionID={2}",_hasAnsweredCorrectly, DateTime.UtcNow ,aSelectedAnswerOption.UserQuestionID));

				BusinessModel.SQL.Execute (_queriesToExecute);
				//Update the userquestion session too
				AppSession.SelectedExamUserQuestionList = BusinessModel.UserQuestion.GetUserQuestionsBySQL (string.Format (
					"SELECT * FROM tblUserQuestion " +
					"WHERE fkUserExamID={0} ORDER BY Sequence", AppSession.SelectedUserExam.UserExamID));

			}
		
		}
	}
}

