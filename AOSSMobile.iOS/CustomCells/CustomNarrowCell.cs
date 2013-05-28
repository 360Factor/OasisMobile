using System;
using System.Drawing;
using MonoTouch.UIKit;
using MonoTouch.Foundation;

namespace OasisMobile.iOS
{
	public class CustomNarrowCell : UITableViewCell
	{

		public CustomNarrowCell (string aReuseIdentifier) : base(UITableViewCellStyle.Default, aReuseIdentifier)
		{

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

		}

	}
}

