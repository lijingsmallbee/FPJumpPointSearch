using System;
using System.Collections.Generic;

using TrueSync;
public interface IPhysicsManagerBase
{
    //
    // Methods
    //
    IWorld GetWorld();

    IWorldClone GetWorldClone();

    void Init();

    void RemoveBody(IBody iBody);

    void UpdateStep();
}