using System;
using System.Drawing;
using MonoTouch.UIKit;
using MonoTouch.Foundation;

namespace OasisMobile.iOS
{
	public class CustomTextFieldCell : UITableViewCell
	{
		private UITextField m_inputTextField;
		private float m_inputTextWidthPct = 60;

		public CustomTextFieldCell (string aReuseIdentifier) : base(UITableViewCellStyle.Default, aReuseIdentifier)
		{
			m_inputTextField = new UITextField ();
			this.AddSubview (m_inputTextField);
		}

		public UITextField InputTextField {
			get {
				return m_inputTextField;
			}
		}

		public float InputTextWidthPct {
			get {
				return m_inputTextWidthPct;
			}
			set {
				m_inputTextWidthPct = value;
			}
		}

		public override void LayoutSubviews ()
		{
			base.LayoutSubviews ();

			SizeF _contentViewSize = ContentView.Frame.Size;
//			SizeF _textLabelSize = new SizeF ();
//			_textLabelSize.Width = ((100 - m_inputTextWidthPct - 5) / (float) 100) * (_contentViewSize.Width - 20);
//			_textLabelSize.Height = TextLabel.Frame.Height;
//
//			TextLabel.Frame = new RectangleF (TextLabel.Frame.Location, _textLabelSize);
////			TextLabel.Frame.Width = ((100 - m_inputTextWidthPct - 5) / (float) 100) * (_contentViewSize.Width - 20);
////			InputTextField.Frame =  new RectangleF ((float)(TextLabel.Frame.X + TextLabel.Frame.Width + (_contentViewSize.Width - 20) *0.05),
////			                                        10,
////			                                        (m_inputTextWidthPct / (float) 100) * (_contentViewSize.Width - 20), 
////			                                        32);
//
			InputTextField.Frame =  new RectangleF (_contentViewSize.Width - 10 - m_inputTextWidthPct / (float)100 * (_contentViewSize.Width-20),
			                                        10,
			                                        (m_inputTextWidthPct / (float) 100) * (_contentViewSize.Width - 20), 
			                                        32);
		}
	}
}

