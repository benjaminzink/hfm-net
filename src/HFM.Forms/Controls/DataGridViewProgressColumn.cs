﻿using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace HFM.Forms.Controls
{
    // Code from: http://social.msdn.microsoft.com/Forums/en-US/winformsdatacontrols/thread/769ca9d6-1e9d-4d76-8c23-db535b2f19c2

    internal class DataGridViewProgressColumn : DataGridViewColumn
    {
        public DataGridViewProgressColumn()
        {
            CellTemplate = new DataGridViewProgressCell();
            SortMode = DataGridViewColumnSortMode.Automatic;
        }
    }

    internal class DataGridViewProgressCell : DataGridViewTextBoxCell
    {
        // Used to make custom cell consistent with a DataGridViewImageCell
        private static readonly Image _EmptyImage = new Bitmap(1, 1, System.Drawing.Imaging.PixelFormat.Format32bppArgb);

        public DataGridViewProgressCell()
        {
            ValueType = typeof(int);
        }

        // Method required to make the Progress Cell consistent with the default Image Cell.
        // The default Image Cell assumes an Image as a value, although the value of the Progress Cell is an int.
        protected override object GetFormattedValue(object value,
                             int rowIndex, ref DataGridViewCellStyle cellStyle,
                             TypeConverter valueTypeConverter,
                             TypeConverter formattedValueTypeConverter,
                             DataGridViewDataErrorContexts context)
        {
            return _EmptyImage;
        }

        protected override void Paint(Graphics g, Rectangle clipBounds, Rectangle cellBounds, int rowIndex, DataGridViewElementStates cellState,
                                      object value, object formattedValue, string errorText, DataGridViewCellStyle cellStyle,
                                      DataGridViewAdvancedBorderStyle advancedBorderStyle, DataGridViewPaintParts paintParts)
        {
            // this is new... sometimes being passed a null
            // value here.  check and get out if so. - 1/28/12
            if (value == null) return;

            var percentage = (float)value;
            var progressVal = Convert.ToInt32(percentage * 100);
            using (var foreColorBrush = new SolidBrush(cellStyle.ForeColor))
            {

                // Draws the cell grid
                base.Paint(g, clipBounds, cellBounds, rowIndex, cellState, value, formattedValue, errorText,
                    cellStyle, advancedBorderStyle, paintParts & ~DataGridViewPaintParts.ContentForeground);

                if (percentage > 0.0)
                {
                    // Draw the progress bar and the text
                    var progressBarColor = Color.FromArgb(163, 189, 242);
                    using (var progressBarBrush = new SolidBrush(progressBarColor))
                    {
                        g.FillRectangle(progressBarBrush, cellBounds.X + 2, cellBounds.Y + 2, Convert.ToInt32((percentage * cellBounds.Width) - 4), cellBounds.Height - 4);
                    }
                    g.DrawString(progressVal + "%", cellStyle.Font, foreColorBrush, cellBounds.X + 6, cellBounds.Y + 2);
                }
                else
                {
                    // draw the text
                    if (DataGridView.CurrentRow != null && DataGridView.CurrentRow.Index == rowIndex)
                    {
                        using (var selectionForeColorBrush = new SolidBrush(cellStyle.SelectionForeColor))
                        {
                            g.DrawString(progressVal + "%", cellStyle.Font, selectionForeColorBrush, cellBounds.X + 6, cellBounds.Y + 2);
                        }
                    }
                    else
                    {
                        g.DrawString(progressVal + "%", cellStyle.Font, foreColorBrush, cellBounds.X + 6, cellBounds.Y + 2);
                    }
                }
            }
        }
    }
}
