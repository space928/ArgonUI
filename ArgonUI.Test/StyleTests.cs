using ArgonUI.Backends.Headless;
using ArgonUI.Styling;
using ArgonUI.UIElements;
using System.Numerics;

namespace ArgonUI.Test;

[TestClass]
public class StyleTests
{
    ArgonManager argon;
    UIWindow wnd;
    Rectangle rect;
    Label label;
    Button btn;
    ContentButton cbtn;
    ContentButton ccbtn;
    Rectangle innerRect;

    private readonly Vector4 colourRed = new(1, 0, 0, 1);
    private readonly Vector4 colourGreen = new(0, 1, 0, 1);
    private readonly Vector4 colourBlue = new(0, 0, 1, 1);
    private readonly Vector4 colourYellow = new(1, 1, 0, 1);
    private readonly Vector4 colourCyan = new(0, 1, 1, 1);
    private readonly Vector4 colourMagenta = new(1, 0, 1, 1);

    /*
     We generate a simple UI hierarchy to apply styles to as shown:

     Root -> Style[Rounding(10), FontSize(30), Colour(Red)]
     |-> Rect
     |-> Label
     |-> Button -> Style[Colour(Green)]
     |   \-> (Content) Label -> Style[Colour(Red)]
     |-> ContentButton
         |-> ContentButton
             |-> Rect

     */
    public StyleTests()
    {
        argon = new();
        wnd = argon.CreateHeadlessWindow();

        wnd.RootElement.Style = new([
            new Style([
                ArgonUIStyles.Rounding(10),
                ArgonUIStyles.FontSize(30),
                ArgonUIStyles.Colour(colourRed)
            ])
        ]);

        // Create some elements
        rect = new Rectangle();
        label = new Label();

        // TODO: Modifying the element tree isn't thread safe
        wnd.RootElement.AddChild(rect);
        rect.Width = 100;
        rect.Height = 100;
        rect.Rounding = 0;
        rect.Colour = colourGreen;

        wnd.RootElement.AddChild(label);
        label.Text = "Hello World!";
        label.FontSize = 40;

        btn = new Button();
        wnd.RootElement.AddChild(btn);
        btn.Text = "ButtonText";
        btn.Style = new([
            new Style([
                ArgonUIStyles.Colour(colourGreen)
            ])
        ]);
        if (btn.Content != null)
        {
            btn.Content.Style = new([
                new Style([
                    ArgonUIStyles.Colour(colourBlue)
                ])
            ]);
        }

        cbtn = new ContentButton();
        wnd.RootElement.AddChild(cbtn);
        ccbtn = new ContentButton();
        cbtn.Content = ccbtn;
        innerRect = new Rectangle();
        ccbtn.Content = innerRect;
    }

    [TestMethod]
    public void TestAddStyleSet()
    {
        Assert.AreEqual(colourRed, cbtn.Colour);          // CBtn
        Assert.AreEqual(colourRed, ccbtn.Colour);         // \-> CBtn
        Assert.AreEqual(colourRed, innerRect.Colour);     //     \-> Rect

        // Add a new style set 
        ccbtn.Style = new([
            new Style([
                ArgonUIStyles.Colour(colourGreen)
            ])
        ]);

        Assert.AreEqual(colourRed, cbtn.Colour);          // CBtn
        Assert.AreEqual(colourGreen, ccbtn.Colour);         // \-> CBtn
        Assert.AreEqual(colourGreen, innerRect.Colour);     //     \-> Rect
    }

    [TestMethod]
    public void TestRemoveStyleSet()
    {
        // Add a new style set 
        ccbtn.Style = new([
            new Style([
                ArgonUIStyles.Colour(colourGreen)
            ])
        ]);

        Assert.AreEqual(colourRed, cbtn.Colour);          // CBtn
        Assert.AreEqual(colourGreen, ccbtn.Colour);         // \-> CBtn
        Assert.AreEqual(colourGreen, innerRect.Colour);     //     \-> Rect

        // Now reset it
        ccbtn.Style = null;

        // Check that it was reset correctly
        Assert.AreEqual(colourRed, cbtn.Colour);          // CBtn
        Assert.AreEqual(colourRed, ccbtn.Colour);         // \-> CBtn
        Assert.AreEqual(colourRed, innerRect.Colour);     //     \-> Rect
    }

    [TestMethod]
    public void TestAddStyle()
    {

    }

    [TestMethod]
    public void TestRemoveStyle()
    {

    }

    [TestMethod]
    public void TestAddStyleProp()
    {

    }

    [TestMethod]
    public void TestChangeStyleProp()
    {

    }

    [TestMethod]
    public void TestRemoveStyleProp()
    {

    }

    [TestMethod]
    public void TestAddSelector()
    {

    }

    [TestMethod]
    public void TestRemoveSelector()
    {

    }

    [TestMethod]
    public void TestUpdateTree()
    {

    }

    [TestMethod]
    public void TestUpdateProperty()
    {

    }
}