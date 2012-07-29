﻿// 
// TextHistoryItem.cs
//  
// Author:
//       Andrew Davis <andrew.3.1415@gmail.com>
// 
// Copyright (c) 2012 Andrew Davis, GSoC 2012
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
// THE SOFTWARE.

using System;
using Cairo;

namespace Pinta.Core
{
	public class TextHistoryItem : BaseHistoryItem
	{
		UserLayer userLayer;

		ImageSurface textSurface;
		ImageSurface userSurface;

		TextEngine tEngine;
		Gdk.Rectangle textBounds;

		/// <summary>
		/// A history item for when text is created, edited, and/or finalized.
		/// </summary>
		/// <param name="icon">The history item's icon.</param>
		/// <param name="text">The history item's title.</param>
		/// <param name="passedTextSurface">The stored TextLayer surface.</param>
		/// <param name="passedUserSurface">The stored UserLayer surface.</param>
		/// <param name="passedUserLayer">The UserLayer being modified.</param>
		public TextHistoryItem(string icon, string text, ImageSurface passedTextSurface, ImageSurface passedUserSurface, UserLayer passedUserLayer) : base(icon, text)
		{
			userLayer = passedUserLayer;

			textSurface = passedTextSurface.Clone();
			userSurface = passedUserSurface.Clone();

			tEngine = userLayer.tEngine.Clone();
			textBounds = new Gdk.Rectangle(userLayer.textBounds.X, userLayer.textBounds.Y, userLayer.textBounds.Width, userLayer.textBounds.Height);
		}

		public TextHistoryItem(string icon, string text) : base(icon, text)
		{
		}

		public override void Undo()
		{
			Swap();
		}

		public override void Redo()
		{
			Swap();
		}

		private void Swap()
		{
			// Grab the original surface
			ImageSurface surf = PintaCore.Workspace.ActiveDocument.CurrentUserLayer.TextLayer.Surface;

			// Undo to the "old" surface
			PintaCore.Workspace.ActiveDocument.CurrentUserLayer.TextLayer.Surface = textSurface;

			// Store the original surface for Redo
			textSurface = surf;



			// Grab the original surface
			surf = PintaCore.Workspace.ActiveDocument.CurrentUserLayer.Surface;

			// Undo to the "old" surface
			PintaCore.Workspace.ActiveDocument.CurrentUserLayer.Surface = userSurface;

			// Store the original surface for Redo
			userSurface = surf;



			//Redraw everything since surfaces were swapped.
			PintaCore.Workspace.Invalidate();



			//Store the old text data temporarily.
			TextEngine oldTEngine = tEngine;
			Gdk.Rectangle oldTextBounds = textBounds;

			//Swap half of the data.
			tEngine = userLayer.tEngine;
			textBounds = userLayer.textBounds;

			//Swap the other half.
			userLayer.tEngine = oldTEngine;
			userLayer.textBounds = oldTextBounds;
		}

		public override void Dispose()
		{
			// Free up native surface
			if (textSurface != null)
				(textSurface as IDisposable).Dispose();

			// Free up native surface
			if (userSurface != null)
				(userSurface as IDisposable).Dispose();
		}

		public void TakeSnapshotOfLayer(UserLayer passedUserLayer)
		{
			userLayer = passedUserLayer;

			textSurface = userLayer.TextLayer.Surface.Clone();
			userSurface = userLayer.Surface.Clone();

			tEngine = userLayer.tEngine.Clone();
			textBounds = new Gdk.Rectangle(userLayer.textBounds.X, userLayer.textBounds.Y, userLayer.textBounds.Width, userLayer.textBounds.Height);
		}
	}
}
