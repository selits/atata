﻿using System;
using System.Reflection;

namespace Atata
{
    public abstract class SpecificTriggerAttribute : TriggerAttribute
    {
        protected SpecificTriggerAttribute(TriggerEvents on, TriggerPriority priority = TriggerPriority.Medium, TriggerScope appliesTo = TriggerScope.Self)
            : base(on, priority, appliesTo)
        {
        }

        public override sealed void Execute<TOwner>(TriggerContext<TOwner> context)
        {
            MethodInfo declaredMethod = GetType().GetMethod("Execute", BindingFlags.DeclaredOnly | BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
            if (declaredMethod != null)
            {
                Type ownerType = context.Owner.GetType();

                MethodInfo genericMethod = declaredMethod.MakeGenericMethod(ownerType);
                genericMethod.Invoke(this, new object[] { context });
            }
        }
    }
}
