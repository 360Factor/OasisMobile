using System;
using System.Drawing;
using MonoTouch.UIKit;
using MonoTouch.Foundation;

namespace OasisMobile.iOS
{
	public class CustomTileButtonCell : UITableViewCell
	{
		public CustomTileButtonCell (string aReuseIdentifier) : base(UITableViewCellStyle.Default,aReuseIdentifier)
		{
			this.BackgroundView = null;
		}

		private UIButton[] m_CellButtons = new UIButton[] {};
		public UIButton[] CellButtons{
			get{
				return m_CellButtons;
			}
			set{
				m_CellButtons = value;
				AddButtonToCellSubview ();
			}
		}

		public void AddButtonToCellSubview(){
			foreach (var x in ContentView.Subviews) {
				x.RemoveFromSuperview ();
			}

			foreach (var _button in m_CellButtons) {
				ContentView.AddSubview (_button);
			}
		}

		public override void LayoutSubviews ()
		{
			base.LayoutSubviews ();
			float _buttonMaxDimension = 90;
			float _targetColumnSpacing = 12;
			if (ContentView.Bounds.Width > 300) {
				//For larger screen, we show more spacing
				_targetColumnSpacing = 30;
			}
			//Formula is ContentWidth = [ButtonMaxDimension]x + ([TargetButtonSpacing]) (x -1) (because there is one less cell spacing than button
			// ->ContentWidth = ([ButtonMaxDimension] + [TargetButtonSpacing])x - [TargetButtonSpacing]
			// ->x = (ContentWidth + [TargetButtonSpacing]) / ([ButtonMaxDimension] + [TargetButtonSpacing])
			int _buttonPerRow = (int) Math.Floor ((ContentView.Bounds.Width + _targetColumnSpacing) / (_buttonMaxDimension + _targetColumnSpacing));
			float _calculatedColumnSpacing = (ContentView.Bounds.Width - (_buttonMaxDimension * _buttonPerRow)) / (_buttonPerRow - 1);

			int _cellButtonCount = m_CellButtons.Length;
			for (int i=0; i<_cellButtonCount; i++) {
				RectangleF _buttonFrame = new RectangleF ();
				int _buttonColumnIndex = i % _buttonPerRow;
				int _buttonRowIndex = (int)i / _buttonPerRow;
				_buttonFrame.X = _buttonColumnIndex * (_buttonMaxDimension + _calculatedColumnSpacing);
				_buttonFrame.Y = _buttonRowIndex * (_buttonMaxDimension + _calculatedColumnSpacing);
				_buttonFrame.Width = _buttonMaxDimension;
				_buttonFrame.Height = _buttonMaxDimension;

				UIButton _buttonToArrange = m_CellButtons [i];
				_buttonToArrange.Frame = _buttonFrame;
			}
		}

	}
}
