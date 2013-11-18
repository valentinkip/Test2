using System.Collections.Generic;
using ManchkinQuest.Core;

namespace ManchkinQuest.UI
{
  class MazeFragmentElement : MazePanelBase
  {
    private readonly IMazeRoomInfo myRoom;

    public MazeFragmentElement(IGame game, IMazeRoomInfo room)
    {
      myRoom = room;
      Game = game;
    }

    protected override IEnumerable<IMazeRoomInfo> Rooms
    {
      get { return new[] {myRoom}; }
    }
  }
}