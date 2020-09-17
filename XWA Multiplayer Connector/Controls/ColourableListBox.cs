using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace XWA_Multiplayer_Connector.Controls
{
    class ColourableListBox : ListBox
    {
        //Fields

        private readonly Dictionary<object, Color> foreColours = new Dictionary<object, Color>();
        
        private readonly Dictionary<object, Color> backColours = new Dictionary<object, Color>();

        /// <summary>
        /// Override the items so that it forces them to use the public methods here
        /// </summary>
        public new object Items = new object();

        //Constructor

        public ColourableListBox()
        {
            DoubleBuffered = true; //prevent flicker, not sure if this is necessary.
            DrawMode = DrawMode.OwnerDrawFixed;
        }

        //Public Methods

        public void AddItemWithColour(object item, Color? backColour = null, Color? foreColour = null)
        {
            base.Items.Add(item);

            if (backColour != null)
            {
                backColours.Add(item, (Color)backColour);
            }

            if (foreColour != null)
            {
                foreColours.Add(item, (Color)foreColour);
            }
        }

        public void ChangeItemColour(object item, Color? backColour = null, Color? foreColour = null)
        {
            if (backColour != null)
            {
                if (backColours.ContainsKey(item))
                {
                    backColours[item] = (Color)backColour;
                }
                else
                {
                    backColours.Add(item, (Color)backColour);
                }
            }

            if (foreColour != null)
            {
                if (foreColours.ContainsKey(item))
                {
                    foreColours[item] = (Color)foreColour;
                }
                else
                {
                    foreColours.Add(item, (Color)foreColour);
                }
            }
        }

        public void RemoveItem(object item)
        {
            base.Items.Remove(item);
        }

        public void RemoveAllItems()
        {
            foreColours.Clear();
        }

        //Overrides

        /// <summary>
        /// /// Heavily inspired from stack overflow:
        /// https://stackoverflow.com/questions/2554609/c-sharp-changing-listbox-row-color
        /// </summary>
        /// <param name="e"></param>
        protected override void OnDrawItem(DrawItemEventArgs e)
        {
            if (e.Index == -1)
            {
                //If index is -1, no customization is necessary
                return;
            }

            //Item to be drawn
            object item = null;

            //Find the item to be drawn
            if (base.Items.Count > e.Index)
            {
                item = base.Items[e.Index];
            }

            //If the item was not found
            if (item == null)
            {
                return;
            }

            //Default forecolor
            Color foreColor = e.ForeColor;
            Color backColor = e.BackColor;

            //If we have a forecolor
            if (foreColours.ContainsKey(item))
            {
                //Only override the forecolour if it's black
                if (e.ForeColor.ToArgb() == Color.Black.ToArgb())
                {
                    foreColor = foreColours[item];
                }
            }

            //If we have a backcolor
            if (backColours.ContainsKey(item))
            {
                //Only override the backcolour if it's not white
                if (e.BackColor.ToArgb() == Color.White.ToArgb())
                {
                    backColor = backColours[item];
                }
            }

            e.DrawBackground();
            Graphics g = e.Graphics;

            // draw the background color you want
            // mine is set to olive, change it to whatever you want
            g.FillRectangle(new SolidBrush(backColor), e.Bounds);

            // draw the text of the list item, not doing this will only show
            // the background color
            // you will need to get the text of item to display
            g.DrawString(item.ToString(), e.Font, new SolidBrush(foreColor), new PointF(e.Bounds.X, e.Bounds.Y));
        }
    }
}
