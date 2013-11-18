﻿using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using ManchkinQuest.Core;

namespace ManchkinQuest.UI
{
  public partial class GameTablePanel
  {
    private IGame myGame;
    private PlaneProjection myPlaneProjection;
    private TranslateTransform myTranslateTransform;
    private ScaleTransform myScaleTransform;
    private TransformData myCurrentTransformData;
    private TransformData mySavedTransformData = null;

    private class TransformData
    {
      public PlaneProjection PlaneProjection { get; private set; }
      public TranslateTransform TranslateTransform { get; private set; }
      public ScaleTransform ScaleTransform { get; private set; }

      public TransformData(PlaneProjection planeProjection, TranslateTransform translateTransform, ScaleTransform scaleTransform)
      {
        PlaneProjection = new PlaneProjection
                              {
                                RotationX = planeProjection.RotationX,
                                RotationY = planeProjection.RotationY,
                                RotationZ = planeProjection.RotationZ,
                                LocalOffsetX = planeProjection.LocalOffsetX,
                                LocalOffsetY = planeProjection.LocalOffsetY,
                                LocalOffsetZ = planeProjection.LocalOffsetZ,
                              };
        TranslateTransform = new TranslateTransform
                                 {
                                   X = translateTransform.X,
                                   Y = translateTransform.Y,
                                 };
        ScaleTransform = new ScaleTransform
                             {
                               ScaleX = scaleTransform.ScaleX,
                               ScaleY = scaleTransform.ScaleY,
                               CenterX = scaleTransform.CenterX,
                               CenterY = scaleTransform.CenterY,
                             };
      }
    }

    public GameTablePanel()
    {
      InitializeComponent();

      Application.Current.RootVisual.KeyDown += delegate(object sender, KeyEventArgs e)
      {
        if (e.Key == Key.Escape && mySavedTransformData != null)
        {
          Animate(mySavedTransformData.TranslateTransform, mySavedTransformData.ScaleTransform, mySavedTransformData.PlaneProjection);
          mySavedTransformData = null;
          e.Handled = true;
        }
      };

      AddEventHandlers(TopPlayerPanel);
      AddEventHandlers(BottomPlayerPanel);
      AddEventHandlers(LeftPlayerPanel);
      AddEventHandlers(RightPlayerPanel);
    }

    protected override Size ArrangeOverride(Size finalSize)
    {
      Projection = myPlaneProjection = new PlaneProjection { RotationX = -30, LocalOffsetZ = -500, LocalOffsetY = -finalSize.Height / 2 };

      const double scale = 1.35;
      myTranslateTransform = new TranslateTransform { X = 0, Y = 0 };
      myScaleTransform = new ScaleTransform { ScaleX = scale, ScaleY = scale, CenterX = finalSize.Width / 2, CenterY = finalSize.Height / 2 };
      var transformGroup = new TransformGroup();
      transformGroup.Children.Add(myTranslateTransform);
      transformGroup.Children.Add(myScaleTransform);
      RenderTransform = transformGroup;
      myCurrentTransformData = new TransformData(myPlaneProjection, myTranslateTransform, myScaleTransform);

      return base.ArrangeOverride(finalSize);
    }

    public void SetGame(IGame game)
    {
      myGame = game;
      Update();
    }

    public void Update()
    {
      int playerIndex = myGame.Players.IndexOf(myGame.ActivePlayer);
      int playersCount = myGame.Players.Count;

      BottomPlayerPanel.Placement = PlayerPanelPlacement.Bottom;
      BottomPlayerPanel.Player = myGame.Players[playerIndex];
      BottomPlayerPanel.IsActivePlayer = true;
      BottomPlayerPanel.Update();

      if (playersCount > 2)
      {
        playerIndex = (playerIndex + 1)%playersCount;
        LeftPlayerPanel.Placement = PlayerPanelPlacement.Left;
        LeftPlayerPanel.Player = myGame.Players[playerIndex];
        LeftPlayerPanel.Update();
      }
      else
      {
        LeftPlayerPanel.Player = null;
      }

      playerIndex = (playerIndex + 1) % playersCount;
      TopPlayerPanel.Placement = PlayerPanelPlacement.Top;
      TopPlayerPanel.Player = myGame.Players[playerIndex];
      TopPlayerPanel.Update();

      if (playersCount > 3)
      {
        playerIndex = (playerIndex + 1) % playersCount;
        RightPlayerPanel.Placement = PlayerPanelPlacement.Right;
        RightPlayerPanel.Player = myGame.Players[playerIndex];
        RightPlayerPanel.Update();
      }
      else
      {
        RightPlayerPanel.Player = null;
      }

      MazePanel.Game = myGame;
    }

    private void AddEventHandlers(PlayerPanel playerPanel)
    {
/*
      playerPanel.MouseEnter += delegate
      {
        if (playerPanel.Player != null)
        {
          playerPanel.Cursor = Cursors.Hand;
          playerPanel.Navigated = true;
        }
      };
      playerPanel.MouseLeave += delegate
      {
        if (playerPanel.Player != null)
        {
          playerPanel.Cursor = Cursors.Arrow;
          playerPanel.Navigated = false;
        }
      };
      playerPanel.MouseLeftButtonUp += delegate
                                         {
                                           if (playerPanel.Player != null)
                                           {
                                             var zoomedPlayerPanel = new PlayerPanel
                                                               {
                                                                 Player = playerPanel.Player,
                                                                 Placement = PlayerPanelPlacement.Bottom/*TODO?♥1♥,
                                                               };
                                             zoomedPlayerPanel.Update();
                                             var grid = new Grid {Background = LayoutRoot.Background};
                                             grid.Children.Add(zoomedPlayerPanel);
                                             UIElement rootVisual = Application.Current.RootVisual;
                                             var rect = new Rect(0, rootVisual.RenderSize.Height / 4, rootVisual.RenderSize.Width, rootVisual.RenderSize.Height / 2);
                                             Rect startRect = playerPanel.GetRect(rootVisual);
                                             UIManager.Instance.ShowInPopup(grid, startRect, rect);
                                           }
                                         };
*/
    }

    public void AnimateZoom(Rect rectToZoom)
    {
      double scaleW = RenderSize.Width / rectToZoom.Width;
      double scaleH = RenderSize.Height / rectToZoom.Height;
      double scale = Math.Min(scaleW, scaleH);
      double rectCenterX = rectToZoom.X + rectToZoom.Width / 2;
      double rectCenterY = rectToZoom.Y + rectToZoom.Height / 2;
      double centerX = RenderSize.Width / 2;
      double centerY = RenderSize.Height / 2;

      mySavedTransformData = AnimateTo(centerX, rectCenterX, centerY, rectCenterY, scale);
    }

    private TransformData AnimateTo(double centerX, double rectCenterX, double centerY, double rectCenterY, double scale)
    {
      var savedTransform = new TransformData(myCurrentTransformData.PlaneProjection, myCurrentTransformData.TranslateTransform, myCurrentTransformData.ScaleTransform);
      var newTranslateTransform = new TranslateTransform {X = centerX - rectCenterX, Y = centerY - rectCenterY};
      var newScaleTransform = new ScaleTransform{ScaleX = scale, ScaleY = scale, CenterX = centerX, CenterY = centerY};
      var newPlaneProjection = new PlaneProjection();
      Animate(newTranslateTransform, newScaleTransform, newPlaneProjection, new PowerEase {Power = 5});
      return savedTransform;
    }

    private void Animate(TranslateTransform newTranslateTransform, ScaleTransform newScaleTransform, PlaneProjection newPlaneProjection, PowerEase ease = null)
    {
      myCurrentTransformData = new TransformData(newPlaneProjection, newTranslateTransform, newScaleTransform);
      UIUtil.StartAnimation(0.5, 
                            new AnimationInfo(myTranslateTransform, new PropertyPath(TranslateTransform.XProperty), myTranslateTransform.X, newTranslateTransform.X, ease),
                            new AnimationInfo(myTranslateTransform, new PropertyPath(TranslateTransform.YProperty), myTranslateTransform.Y, newTranslateTransform.Y, ease),
                            new AnimationInfo(myScaleTransform, new PropertyPath(ScaleTransform.ScaleXProperty), myScaleTransform.ScaleX, newScaleTransform.ScaleX, ease),
                            new AnimationInfo(myScaleTransform, new PropertyPath(ScaleTransform.ScaleYProperty), myScaleTransform.ScaleY, newScaleTransform.ScaleY, ease),
                            new AnimationInfo(myScaleTransform, new PropertyPath(ScaleTransform.CenterXProperty), myScaleTransform.CenterX, newScaleTransform.CenterX, ease),
                            new AnimationInfo(myScaleTransform, new PropertyPath(ScaleTransform.CenterYProperty), myScaleTransform.CenterY, newScaleTransform.CenterY, ease),
                            new AnimationInfo(myPlaneProjection, new PropertyPath(PlaneProjection.RotationXProperty), myPlaneProjection.RotationX, newPlaneProjection.RotationX, ease),
                            new AnimationInfo(myPlaneProjection, new PropertyPath(PlaneProjection.LocalOffsetZProperty), myPlaneProjection.LocalOffsetZ, newPlaneProjection.LocalOffsetZ, ease),
                            new AnimationInfo(myPlaneProjection, new PropertyPath(PlaneProjection.LocalOffsetYProperty), myPlaneProjection.LocalOffsetY, newPlaneProjection.LocalOffsetY, ease));
    }
  }
}
