#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.Animations;
using UnityEngine;
using ShooterBoss.Boss.StateBehaviours;

namespace ShooterBoss.EditorTools
{
    /// <summary>
    /// Gera o AnimatorController do chefão com os parâmetros "lifeState" (int)
    /// e "invulnerable" (bool), 9 estados (um por comportamento) e as
    /// transições entre eles, já com a StateMachineBehaviour correta anexada
    /// em cada estado. Rode pelo menu Tools > ShooterBoss > Build Boss Animator.
    /// </summary>
    public static class BossAnimatorBuilder
    {
        private const string OutputPath = "Assets/Animations/BossController.controller";

        [MenuItem("Tools/ShooterBoss/Build Boss Animator")]
        public static void Build()
        {
            System.IO.Directory.CreateDirectory("Assets/Animations");

            var controller = AnimatorController.CreateAnimatorControllerAtPath(OutputPath);

            controller.AddParameter(
                "lifeState",
                AnimatorControllerParameterType.Int
            );

            controller.AddParameter(
                "invulnerable",
                AnimatorControllerParameterType.Bool
            );

            var rootSM = controller.layers[0].stateMachine;

            // Um sub-state machine por nível de vida,
            // com 3 estados (A/B/C) dentro.
            var high = AddLifeStateGroup(
                rootSM,
                "VidaAlta",
                0,
                typeof(Boss_HighA),
                typeof(Boss_HighB),
                typeof(Boss_HighC)
            );

            var medium = AddLifeStateGroup(
                rootSM,
                "VidaMedia",
                1,
                typeof(Boss_MediumA),
                typeof(Boss_MediumB),
                typeof(Boss_MediumC)
            );

            var low = AddLifeStateGroup(
                rootSM,
                "VidaBaixa",
                2,
                typeof(Boss_LowA),
                typeof(Boss_LowB),
                typeof(Boss_LowC)
            );

            // Qualquer estado -> troca de grupo assim que lifeState muda.
            // "invulnerable" segura a transição enquanto estiver ativo.
            LinkGroups(rootSM, high, medium, low);

            // Primeiro estado do grupo VidaAlta.
            rootSM.defaultState = high.entryState;

            EditorUtility.SetDirty(controller);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();

            Debug.Log(
                $"BossController gerado em {OutputPath}. " +
                "Arraste-o no Animator do chefão."
            );
        }

        private class LifeGroup
        {
            // Agora é AnimatorState diretamente, e não ChildAnimatorState.
            public AnimatorState entryState;

            // Estados A, B e C.
            public AnimatorState[] states;
        }

        private static LifeGroup AddLifeStateGroup(
            AnimatorStateMachine root,
            string groupName,
            int lifeStateValue,
            System.Type behaviourA,
            System.Type behaviourB,
            System.Type behaviourC)
        {
            var sub = root.AddStateMachine(groupName);

            var a = sub.AddState(groupName + "_A");
            var b = sub.AddState(groupName + "_B");
            var c = sub.AddState(groupName + "_C");

            // Adiciona os comportamentos aos estados.
            a.AddStateMachineBehaviour(behaviourA);
            b.AddStateMachineBehaviour(behaviourB);
            c.AddStateMachineBehaviour(behaviourC);

            // Ciclo interno A -> B -> C -> A.
            AddCycleTransition(a, b);
            AddCycleTransition(b, c);
            AddCycleTransition(c, a);

            // Estado padrão do sub-state machine.
            sub.defaultState = a;

            return new LifeGroup
            {
                entryState = a,
                states = new[] { a, b, c }
            };
        }

        private static void AddCycleTransition(
            AnimatorState from,
            AnimatorState to)
        {
            var t = from.AddTransition(to);

            t.hasExitTime = true;
            t.exitTime = 1f;
            t.duration = 0f;
            t.hasFixedDuration = true;
        }

        private static void LinkGroups(
            AnimatorStateMachine root,
            LifeGroup high,
            LifeGroup medium,
            LifeGroup low)
        {
            // Vida Alta -> Vida Média
            LinkAllStatesTo(
                high.states,
                medium.entryState,
                1,
                0
            );

            // Vida Alta -> Vida Baixa
            LinkAllStatesTo(
                high.states,
                low.entryState,
                2,
                0
            );

            // Vida Média -> Vida Baixa
            LinkAllStatesTo(
                medium.states,
                low.entryState,
                2,
                1
            );

            // Vida Média -> Vida Alta
            LinkAllStatesTo(
                medium.states,
                high.entryState,
                0,
                1
            );

            // Vida Baixa -> Vida Alta
            LinkAllStatesTo(
                low.states,
                high.entryState,
                0,
                2
            );

            // Vida Baixa -> Vida Média
            LinkAllStatesTo(
                low.states,
                medium.entryState,
                1,
                2
            );
        }

        private static void LinkAllStatesTo(
            AnimatorState[] fromStates,
            AnimatorState to,
            int lifeStateEquals,
            int currentGroupValue)
        {
            foreach (var from in fromStates)
            {
                var t = from.AddTransition(to);

                t.hasExitTime = false;
                t.duration = 0f;
                t.hasFixedDuration = true;

                t.AddCondition(
                    AnimatorConditionMode.Equals,
                    lifeStateEquals,
                    "lifeState"
                );

                t.AddCondition(
                    AnimatorConditionMode.IfNot,
                    0,
                    "invulnerable"
                );
            }
        }
    }
}
#endif
