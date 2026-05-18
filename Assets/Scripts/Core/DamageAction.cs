using Unity.VisualScripting;
using UnityEngine;

public class DamageAction
{
    public static DamageAction Instance;

    private enum DamageActionPhase
    {
        Disabled,
        Waiting,
        Started,
        Performed,
        Canceled
    }

    private DamageActionPhase m_Phase;


    public DamageAction()
    {
        m_Phase = DamageActionPhase.Started;
    }



    public static DamageAction GetActionOrNull()
    {
        if (Instance == null)
        {
            return null;
        }

        if (Instance.m_Phase == DamageActionPhase.Disabled || Instance.m_Phase == DamageActionPhase.Waiting)
        {
            return null;
        }

        return new DamageAction();
    }


    public struct CallbackContext
    {
        private DamageActionPhase phase;


        public bool started => phase == DamageActionPhase.Started;
        public bool performed => phase == DamageActionPhase.Performed;
        public bool canceled => phase == DamageActionPhase.Canceled;
    }
}