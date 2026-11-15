using UnityEngine;

public interface IPrototipo<T>
{
    T Clonar(Vector3 posicion, Quaternion rotacion);
}