
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
		private BusinessModel.UserQuestion m_userQuestionToDisplay;

		public Question_iPhone (BusinessModel.UserQuestion aUserQuestion) : base ("Question_iPhone", null)
		{
			m_userQuestionToDisplay = aUserQuestion;
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
			this.Title = string.Format ("{0} of {1}", m_userQuestionToDisplay.Sequence,
			                            AppSession.SelectedExamUserQuestionList.Count);

			tblvQuestion.Source = new Question_iPhoneTableSource (m_userQuestionToDisplay, this);
//			tblvQuestion.SelectRow (NSIndexPath.FromRowSection (1,1),
//			                                                false,UITableViewScrollPosition.None);
		}

		public void DisplayUserQuestion (BusinessModel.UserQuestion aUserQuestion)
		{
			m_userQuestionToDisplay = aUserQuestion;

			this.Title = string.Format ("{0} of {1}", m_userQuestionToDisplay.Sequence,
			                            AppSession.SelectedExamUserQuestionList.Count);

			tblvQuestion.Source = new Question_iPhoneTableSource (m_userQuestionToDisplay, this);
			tblvQuestion.ReloadData ();
		}

		public class Question_iPhoneTableSource : UITableViewSource
		{
			public enum SubmittedQuestionViewSections
			{
				QuestionStemAndImages = 0,
				QuestionAnswerOptions = 1,
				QuestionCommentary = 2,
				QuestionReferences = 3
			}

			public enum UnsubmittedQuestionViewSections
			{
				QuestionStemAndImages = 0,
				QuestionAnswerOptions = 1,
				SubmitAnswerButton = 2
			}

			private Question_iPhone m_currentViewController;
			private BusinessModel.UserQuestion  m_userQuestion;
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

				BusinessModel.UserAnswerOptionDetail _selectedAnswer = (from x in m_questionAnswerOptions where x.IsSelected select x).FirstOrDefault();

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
								cell = new UITableViewCell (UITableViewCellStyle.Default, "imageCell");
							}
							UIImage _imageAtRow = UIImage.FromFile (m_questionImages [indexPath.Row - 1].FilePath);
							float _widthToHeightRatio = _imageAtRow.Size.Width / _imageAtRow.Size.Height;
							float _maxDimension = tableView.Bounds.Width - 40;
							SizeF _imageSize;
							PointF _startingLocation;
							if (_widthToHeightRatio >= 1) {
								_imageSize = new SizeF (_maxDimension, (int)(_maxDimension / _widthToHeightRatio));
								_startingLocation = new PointF (10, 10);
							} else {
								int _imageWidth = (int)(_widthToHeightRatio * _maxDimension);
								_imageSize = new SizeF (_imageWidth, _maxDimension);
								_startingLocation = new PointF ((cell.ContentView.Frame.Width - _imageWidth) / 2, 0);
							}
							UIImageView _imageView = new UIImageView (new RectangleF (_startingLocation, _imageSize));
							_imageView.Image = _imageAtRow;
							foreach (UIView _subview in cell.ContentView.Subviews) {
								_subview.RemoveFromSuperview ();
							}
							cell.ContentView.AddSubview (_imageView);
						}
						break;
					case (int)SubmittedQuestionViewSections.QuestionAnswerOptions:
						cell = tableView.DequeueReusableCell ("cell");
						if (cell == null) {
							cell = new UITableViewCell (UITableViewCellStyle.Default, "cell");
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
								cell = new UITableViewCell (UITableViewCellStyle.Default, "imageCell");
							}
							UIImage _imageAtRow = UIImage.FromFile (m_questionImages [indexPath.Row - 1].FilePath);
							float _widthToHeightRatio = _imageAtRow.Size.Width / _imageAtRow.Size.Height;
							float _maxDimension = cell.ContentView.Frame.Width - 40;
							SizeF _imageSize;
							PointF _startingLocation;
							if (_widthToHeightRatio >= 1) {
								_imageSize = new SizeF (_maxDimension, (int)(_maxDimension / _widthToHeightRatio));
								_startingLocation = new PointF (10, 10);
							} else {
								int _imageWidth = (int)(_widthToHeightRatio * _maxDimension);
								_imageSize = new SizeF (_imageWidth, _maxDimension);
								_startingLocation = new PointF ((cell.ContentView.Frame.Width - _imageWidth) / 2, 0);
							} 
							UIImageView _imageView = new UIImageView (new RectangleF (_startingLocation, _imageSize));
							_imageView.Image = _imageAtRow;

							foreach (UIView _subview in cell.ContentView.Subviews) {
								_subview.RemoveFromSuperview ();
							}
							cell.ContentView.AddSubview (_imageView);
						}
						break;
					case (int)UnsubmittedQuestionViewSections.QuestionAnswerOptions:
						cell = tableView.DequeueReusableCell ("cell");
						if (cell == null) {
							cell = new UITableViewCell (UITableViewCellStyle.Default, "cell");
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
						btnSubmitQuestionAnswer.TouchUpInside+=btnSubmitQuestionAnswer_Click;

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
				if(m_showQuestionAnswer)
				{
					return null;
				}else{
					if(indexPath.Section == (int)UnsubmittedQuestionViewSections.QuestionAnswerOptions){
						return indexPath;
					}else{
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
						BusinessModel.UserAnswerOptionDetail _selectedAnswerOption = m_questionAnswerOptions [indexPath.Row];
						//Update the source global variable so we can pull the answer data later on
						for (int i=0; i<m_questionAnswerOptions.Count; i++) {
							BusinessModel.UserAnswerOptionDetail _answerOptionObj = m_questionAnswerOptions [i];
							if (i == indexPath.Row) {
								_answerOptionObj.IsSelected = true;
							} else {
								_answerOptionObj.IsSelected = false;
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
						if(m_questionAnswerOptions[indexPath.Row].IsSelected){
							cell.Selected = true;
						}
					} else {
						cell.SelectionStyle = UITableViewCellSelectionStyle.None;
					}
				}
				
			}

			private void btnSubmitQuestionAnswer_Click (object sender, EventArgs e)
			{
				BusinessModel.UserAnswerOptionDetail _selectedAnswerOption = 
					(from x in m_questionAnswerOptions where x.IsSelected select x).FirstOrDefault ();
				if (_selectedAnswerOption == null) {
					UIAlertView _requiredFieldAlert = new UIAlertView ("Selection Required", "Please select an answer before continuing", null, "Ok", null);
					_requiredFieldAlert.Show ();
					return;
				}
				int _hasAnsweredCorrectly;
				if(_selectedAnswerOption.IsCorrect){
					_hasAnsweredCorrectly = 1;
				}else{
					_hasAnsweredCorrectly = 0;
				}

				List<string> _queriesToExecute = new List<string>();

				_queriesToExecute.Add (string.Format (
					"UPDATE UserAnswerOption SET IsSelected=1 WHERE pkUserAnswerOptionID={0}", 
					_selectedAnswerOption.UserAnswerOptionID));
				_queriesToExecute.Add (string.Format (
					"UPDATE UserAnswerOption SET IsSelected=0 " +
					"WHERE pkUserAnswerOptionID!={0} AND fkUserQuestionID={1}", 
					_selectedAnswerOption.UserAnswerOptionID, _selectedAnswerOption.UserQuestionID));
				_queriesToExecute.Add (string.Format (
					"UPDATE UserQuestion SET HasAnswered=1, HasAnsweredCorrectly={0} " +
					"WHERE pkUserQuestionID={1}",_hasAnsweredCorrectly,_selectedAnswerOption.UserQuestionID));
			
					BusinessModel.SQL.Execute (_queriesToExecute);
				//Update the userquestion session too
				AppSession.SelectedExamUserQuestionList = BusinessModel.UserQuestion.GetUserQuestionsBySQL (string.Format (
					"SELECT * FROM UserQuestion " +
					"WHERE fkUserExamID={0} ORDER BY Sequence", AppSession.SelectedUserExam.UserExamID));

				BusinessModel.UserQuestion _updatedUserQuestion = 
					(from x in AppSession.SelectedExamUserQuestionList 
					 where x.UserQuestionID == m_userQuestion.UserQuestionID select x).FirstOrDefault();

				if(AppSession.SelectedUserExam.IsLearningMode){
					//For learning mode, we update the interface to show the answer
					m_currentViewController.DisplayUserQuestion (_updatedUserQuestion);
				}else{
					//For examination mode, we go to the next answer
					int _currentQuestionIndex = AppSession.SelectedExamUserQuestionList.IndexOf (_updatedUserQuestion);
					if(_currentQuestionIndex < AppSession.SelectedExamUserQuestionList.Count - 1){
						m_currentViewController.DisplayUserQuestion (
							AppSession.SelectedExamUserQuestionList[_currentQuestionIndex +1]);
					}else{
						m_currentViewController.NavigationController.PopViewControllerAnimated (true);
					}
				}
			}

		}
	}
}

