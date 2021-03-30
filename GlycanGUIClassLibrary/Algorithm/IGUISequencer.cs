using System;
using System.Collections.Generic;
using System.Text;

namespace GlycanGUIClassLibrary.Algorithm
{
    public interface IGUISequencer
    {
        // gui sequence should be increment
        // so, once we get a sequence of a list of all possible gui,
        // we need to choose the most likely gui at each point of the sequence.
        List<GUI> Choose(List<List<GUI>> guis);
    }
}
