using System;
using System.Drawing;
using System.Windows.Forms;
using System.Collections.Generic;
using CharlesLinuxWinFormDesigner.GUI.Fake;
using CharlesLinuxWinFormDesigner.GUI.Fake.Controls;
namespace CharlesLinuxWinFormDesigner.GUI.Fake
{
    public static class FakeControlBuilder
    {

        public static FakeControlContainer BuildForm()
        {
            return new FakeForm();
        }

        public static FakeControl BuildButton()
        {
            return new FakeButton();
        }
        public static FakeControl BuildLabel()
        {
            return new FakeLabel();
        }
        public static FakeControl BuildCheckBox()
        {
            return new FakeCheckBox();
        }
        public static FakeControl BuildRadioButton()
        {
            return new FakeRadioButton();
        }
        public static FakeControl BuildTextBox()
        {
            return new FakeTextBox();
        }
        public static FakeControl BuildPictureBox()
        {
            return new FakePictureBox();
        }
        public static FakeControl BuildNumericUpDown()
        {
            return new FakeNumericUpDown();
        }
        public static FakeControl BuildComboBox()
        {
            return new FakeComboBox();
        }
        public static FakeControl BuildPanel()
        {
            return new FakePanel();
        }
        public static FakeControl BuildGroupBox()
        {
            return new FakeGroupBox();
        }
        public static FakeControl BuildListBox()
        {
            return new FakeListBox();
        }
        public static FakeControl BuildTabControl()
        {
            return new FakeTabControl();
        }
        public static FakeControl BuildTabPage()
        {
            return new FakeTabPage();
        }
        public static FakeControl BuildMenuStrip()
        {
            return new FakeMenuStrip();
        }
        public static FakeControl BuildToolStripMenuItem()
        {
            return new FakeToolStripMenuItem();
        }
        public static FakeControl BuildStatusStrip()
        {
            return new FakeStatusStrip();
        }
        public static FakeControl BuildToolStripStatusLabel()
        {
            return new FakeToolStripStatusLabel();
        }
        public static FakeControl BuildDateTimePicker()
        {
            return new FakeDateTimePicker();
        }
        public static FakeControl BuildFlowLayoutPanel()
        {
            return new FakeFlowLayoutPanel();
        }

    }
}
