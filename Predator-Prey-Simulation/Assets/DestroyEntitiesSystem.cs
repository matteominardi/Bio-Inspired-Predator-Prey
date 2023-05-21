using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;

public partial class DestroyEntitiesSystem : SystemBase
{
    protected override void OnUpdate()
    {        
        Entities
            .WithAll<PreyTag>()
            .WithStructuralChanges()
            .ForEach((Entity e, in PreyComponent preyComponent) => {

                if (preyComponent.Alive == false)
                {
                    EntityManager.DestroyEntity(e);
                    SceneInitializerECS.NUMPREY--;
                }
            }).Run();

        Entities
            .WithAll<PredatorTag>()
            .WithStructuralChanges()
            .ForEach((Entity e, in PredatorComponent predatorComponent) => {

                if (predatorComponent.Alive == false || predatorComponent.Energy <= 0)
                {
                    EntityManager.DestroyEntity(e);
                    SceneInitializerECS.NUMPREDATOR--;
                }
            }).Run();
    }
}
