using ArgonUI.Backends.Headless;
using ArgonUI.Styling;
using ArgonUI.Styling.Selectors;
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
        //rect.Colour = colourGreen;

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
        // Add a new style set 
        ccbtn.Style = new();
        Assert.AreEqual(colourRed, cbtn.Colour);          // CBtn
        Assert.AreEqual(colourRed, ccbtn.Colour);         // \-> CBtn
        Assert.AreEqual(colourRed, innerRect.Colour);     //     \-> Rect

        // Add a style
        ccbtn.Style.Add(
            new Style([
                ArgonUIStyles.Colour(colourGreen)
            ])
        );

        Assert.AreEqual(colourRed, cbtn.Colour);          // CBtn
        Assert.AreEqual(colourGreen, ccbtn.Colour);         // \-> CBtn
        Assert.AreEqual(colourGreen, innerRect.Colour);     //     \-> Rect
    }

    [TestMethod]
    public void TestRemoveStyle()
    {
        // Set a default inner padding on the root
        wnd.RootElement!.Style!.Add(new([ArgonUIStyles.InnerPadding(colourGreen)]));
        // Add a new style set 
        ccbtn.Style = new([
            new Style([
                ArgonUIStyles.Colour(colourGreen)
            ]),
            new Style([
                ArgonUIStyles.InnerPadding(colourRed)
            ]),
        ]);

        Assert.AreEqual(colourRed, cbtn.Colour);          // CBtn
        Assert.AreEqual(colourGreen, ccbtn.Colour);         // \-> CBtn
        Assert.AreEqual(colourGreen, innerRect.Colour);     //     \-> Rect

        Assert.AreEqual((Thickness)colourGreen, cbtn.InnerPadding);
        Assert.AreEqual((Thickness)colourRed, ccbtn.InnerPadding);
        //Assert.AreEqual(colourRed, innerRect.InnerPadding);

        // Remove just the style, not the style set
        ccbtn.Style.RemoveAt(0);

        // Check colour were reset
        Assert.AreEqual(colourRed, cbtn.Colour);          // CBtn
        Assert.AreEqual(colourRed, ccbtn.Colour);         // \-> CBtn
        Assert.AreEqual(colourRed, innerRect.Colour);     //     \-> Rect
        // Check inner padding was not changed
        Assert.AreEqual((Thickness)colourGreen, cbtn.InnerPadding);
        Assert.AreEqual((Thickness)colourRed, ccbtn.InnerPadding);
        //Assert.AreEqual(colourRed, innerRect.InnerPadding);
    }

    [TestMethod]
    public void TestAddStyleProp()
    {
        // Set a default inner padding on the root
        wnd.RootElement!.Style!.Add(new([ArgonUIStyles.InnerPadding(colourGreen)]));
        // Add a style to the cbtn
        cbtn.Style = new(new Style([ArgonUIStyles.Colour(colourBlue)]));

        Assert.AreEqual((Thickness)colourGreen, wnd.RootElement.InnerPadding);
        Assert.AreEqual((Thickness)colourGreen, btn.InnerPadding);
        Assert.AreEqual((Thickness)colourGreen, cbtn.InnerPadding);
        Assert.AreEqual((Thickness)colourGreen, ccbtn.InnerPadding);

        // Now add the inner padding property to the cbtn
        cbtn.Style[0].Add(ArgonUIStyles.InnerPadding(colourRed));

        Assert.AreEqual((Thickness)colourGreen, wnd.RootElement.InnerPadding);
        Assert.AreEqual((Thickness)colourGreen, btn.InnerPadding);
        Assert.AreEqual((Thickness)colourRed, cbtn.InnerPadding);
        Assert.AreEqual((Thickness)colourRed, ccbtn.InnerPadding);
    }

    [TestMethod]
    public void TestChangeStyleProp()
    {
        // Set a default inner padding on the root
        wnd.RootElement!.Style!.Add(new([ArgonUIStyles.InnerPadding(colourGreen)]));
        // Add a style to the cbtn
        cbtn.Style = new(new Style([ArgonUIStyles.InnerPadding(colourBlue)]));

        Assert.AreEqual((Thickness)colourGreen, wnd.RootElement.InnerPadding);
        Assert.AreEqual((Thickness)colourGreen, btn.InnerPadding);
        Assert.AreEqual((Thickness)colourBlue, cbtn.InnerPadding);
        Assert.AreEqual((Thickness)colourBlue, ccbtn.InnerPadding);

        // Now change the inner padding property to the cbtn
        cbtn.Style[0][nameof(ArgonUIStyles.InnerPadding)].Value = (Thickness)colourRed;

        Assert.AreEqual((Thickness)colourGreen, wnd.RootElement.InnerPadding);
        Assert.AreEqual((Thickness)colourGreen, btn.InnerPadding);
        Assert.AreEqual((Thickness)colourRed, cbtn.InnerPadding);
        Assert.AreEqual((Thickness)colourRed, ccbtn.InnerPadding);
    }

    [TestMethod]
    public void TestRemoveStyleProp()
    {
        // Set a default inner padding on the root
        wnd.RootElement!.Style!.Add(new([ArgonUIStyles.InnerPadding(colourGreen)]));
        // Add a style to the cbtn
        cbtn.Style = new(new Style([ArgonUIStyles.InnerPadding(colourBlue)]));

        Assert.AreEqual((Thickness)colourGreen, wnd.RootElement.InnerPadding);
        Assert.AreEqual((Thickness)colourGreen, btn.InnerPadding);
        Assert.AreEqual((Thickness)colourBlue, cbtn.InnerPadding);
        Assert.AreEqual((Thickness)colourBlue, ccbtn.InnerPadding);

        // Now change the inner padding property to the cbtn
        Assert.IsTrue(cbtn.Style[0].Remove(nameof(ArgonUIStyles.InnerPadding)));

        Assert.AreEqual((Thickness)colourGreen, wnd.RootElement.InnerPadding);
        Assert.AreEqual((Thickness)colourGreen, btn.InnerPadding);
        Assert.AreEqual((Thickness)colourGreen, cbtn.InnerPadding);
        Assert.AreEqual((Thickness)colourGreen, ccbtn.InnerPadding);
    }

    [TestMethod]
    public void TestAddSelector()
    {
        Assert.AreEqual(colourRed, rect.Colour);
        Assert.AreEqual(colourRed, cbtn.Colour);
        Assert.AreEqual(colourRed, ccbtn.Colour);
        Assert.AreEqual(colourRed, innerRect.Colour);

        wnd.RootElement.Style!.Add(new(new TypeSelector(typeof(Rectangle)), ArgonUIStyles.Colour(colourCyan)));

        Assert.AreEqual(colourCyan, rect.Colour);
        Assert.AreEqual(colourRed, cbtn.Colour);
        Assert.AreEqual(colourRed, ccbtn.Colour);
        Assert.AreEqual(colourCyan, innerRect.Colour);
    }

    [TestMethod]
    public void TestRemoveSelector()
    {
        wnd.RootElement.Style!.Add(new(new TypeSelector(typeof(Rectangle)), ArgonUIStyles.Colour(colourCyan)));

        Assert.AreEqual(colourCyan, rect.Colour);
        Assert.AreEqual(colourRed, cbtn.Colour);
        Assert.AreEqual(colourRed, ccbtn.Colour);
        Assert.AreEqual(colourCyan, innerRect.Colour);

        wnd.RootElement.Style!.RemoveAt(wnd.RootElement.Style!.Count - 1);

        Assert.AreEqual(colourRed, rect.Colour);
        Assert.AreEqual(colourRed, cbtn.Colour);
        Assert.AreEqual(colourRed, ccbtn.Colour);
        Assert.AreEqual(colourRed, innerRect.Colour);
    }

    [TestMethod]
    public void TestUpdateTree()
    {
        Assert.AreEqual(colourRed, cbtn.Colour);          // CBtn
        Assert.AreEqual(colourRed, ccbtn.Colour);         // \-> CBtn
        Assert.AreEqual(colourRed, innerRect.Colour);     //     \-> Rect

        // Remove a subtree
        Assert.IsTrue(cbtn.RemoveChild(ccbtn));

        // Check that it no longer responds to style changes in it's former parent
        wnd.RootElement.Style![0][nameof(ArgonUIStyles.Colour)].Value = colourBlue;

        Assert.AreEqual(colourBlue, cbtn.Colour);
        Assert.AreEqual(colourRed, ccbtn.Colour);
        Assert.AreEqual(colourRed, innerRect.Colour);

        // Reset the colour
        wnd.RootElement.Style![0][nameof(ArgonUIStyles.Colour)].Value = colourRed;

        // A new ContentButton to the tree
        var ncbtn = new ContentButton();
        cbtn.AddChild(ncbtn);
        // Add the ccbtn as a child of the new content button
        ncbtn.AddChild(ccbtn);
        // The UI tree is now as follows:
        // Root -> Style (RED)
        // |-> CBtn
        //     |-> NCBtn
        //         |-> CCBtn
        //             |-> InnerRect

        // Check all of them are RED
        Assert.AreEqual(colourRed, cbtn.Colour);          // CBtn
        Assert.AreEqual(colourRed, ncbtn.Colour);         // \-> CBtn
        Assert.AreEqual(colourRed, ccbtn.Colour);         //     \-> CBtn
        Assert.AreEqual(colourRed, innerRect.Colour);     //         \-> Rect
    }

    [TestMethod]
    public void TestTagSelector()
    {
        Assert.AreEqual(colourRed, rect.Colour);
        Assert.AreEqual(colourRed, cbtn.Colour);
        Assert.AreEqual(colourRed, ccbtn.Colour);
        Assert.AreEqual(colourRed, innerRect.Colour);

        rect.Tags.AddRange(["cyan", "rect"]);
        innerRect.Tags.AddRange(["cyan", "rect"]);
        cbtn.Tags.AddRange(["cyan"]);

        wnd.RootElement.Style!.Add(new(new TagSelector("cyan", "rect"), ArgonUIStyles.Colour(colourCyan)));

        Assert.AreEqual(colourCyan, rect.Colour);
        Assert.AreEqual(colourRed, cbtn.Colour);
        Assert.AreEqual(colourRed, ccbtn.Colour);
        Assert.AreEqual(colourCyan, innerRect.Colour);
    }

    [TestMethod]
    public void TestUpdateProperty()
    {
        wnd.RootElement.Style!.Add(new(new TagSelector("cyan", "rect"), ArgonUIStyles.Colour(colourCyan)));

        Assert.AreEqual(colourRed, rect.Colour);
        Assert.AreEqual(colourRed, cbtn.Colour);
        Assert.AreEqual(colourRed, ccbtn.Colour);
        Assert.AreEqual(colourRed, innerRect.Colour);

        rect.Tags.AddRange(["cyan", "rect"]);
        innerRect.Tags.AddRange(["cyan", "rect"]);
        cbtn.Tags.AddRange(["cyan"]);

        Assert.AreEqual(colourCyan, rect.Colour);
        Assert.AreEqual(colourRed, cbtn.Colour);
        Assert.AreEqual(colourRed, ccbtn.Colour);
        Assert.AreEqual(colourCyan, innerRect.Colour);

        ((TagSelector)wnd.RootElement.Style![^1].Selector!).Remove("rect");

        Assert.AreEqual(colourCyan, rect.Colour);
        Assert.AreEqual(colourCyan, cbtn.Colour);
        Assert.AreEqual(colourRed, ccbtn.Colour);
        Assert.AreEqual(colourCyan, innerRect.Colour);
    }
}