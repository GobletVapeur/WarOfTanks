using System;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;

[UxmlObject]
public partial class Custom_Binding_control:CustomBinding
{
    public enum Device
    {
        Keyboard,
        Gamepad
    }

    [UxmlAttribute]
    public Device device;

    public Custom_Binding_control()
    {
        updateTrigger = BindingUpdateTrigger.OnSourceChanged;
    }
    protected override void OnDataSourceChanged(in DataSourceContextChanged context)
    {
        VisualElement element = context.targetElement;

        object data = element.dataSource;
        if (element.dataSource == null)
        {
            DataSourceContext parentContext = element.GetHierarchicalDataSourceContext();
            data = parentContext.dataSource;
        }

        if(data != null)
        {
            if(data is InputAction inputAction)
            {
                InputBinding binding = GetInpuTBinding(device, inputAction);
                string value = "";
                if(binding.isComposite)
                {
                    value = binding.name;
                }
                else 
                {
                    value = binding.ToDisplayString();
                
                }
                ConverterGroups.TrySetValueGlobal(ref element, context.bindingId, value, out var errorCode);
            }
        }
    }

    private InputBinding GetInpuTBinding(Device device, InputAction inputAction)
    {
        var bindings = inputAction.bindings;
        for (int i = 0; i < bindings.Count; i++)
        {
            var binding = bindings[i];
            if (binding.isPartOfComposite) continue;

            if (binding.isComposite)
            {
                // Inspect child parts of the composite to detect which device they belong to
                int j = i + 1;
                while (j < bindings.Count && bindings[j].isPartOfComposite)
                {
                    var part = bindings[j];
                    if (device == Device.Keyboard && (part.groups?.Contains("Keyboard") == true || part.path.Contains("<Keyboard>")))
                        return binding;
                    if (device == Device.Gamepad && (part.groups?.Contains("Gamepad") == true || part.path.Contains("<Gamepad>")))
                        return binding;
                    j++;
                }

            }

            if (device == Device.Keyboard && (binding.groups?.Contains("Keyboard") == true || binding.path.Contains("<Keyboard>")))
                return binding;
            if (device == Device.Gamepad && (binding.groups?.Contains("Gamepad") == true || binding.path.Contains("<Gamepad>")))
                return binding;
        }

        return new InputBinding();
    }
}
