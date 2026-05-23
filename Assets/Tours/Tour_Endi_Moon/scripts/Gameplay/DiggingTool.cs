using UnityEngine;

namespace MoonGame
{
    /// <summary>
    /// Лопата. Вешается на модель лопаты.
    /// Единственная функция: при касании с SandPile регистрирует удар → кучка погружается,
    /// затем артефакт появляется на поверхности.
    /// Требует Collider с isTrigger = true на наконечнике лопаты.
    /// </summary>
    public class DiggingTool : MonoBehaviour
    {
        [Header("Минимальный интервал между касаниями одного объекта (сек)")]
        [SerializeField] private float hitCooldown = 0.25f;

        private readonly System.Collections.Generic.Dictionary<int, float> lastHitTime = new();

        private void OnTriggerEnter(Collider other)
        {
            RegisterPileHit(other);
        }

        private void OnTriggerStay(Collider other)
        {
            RegisterPileHit(other);
        }

        private void RegisterPileHit(Collider other)
        {
            var pile = other.GetComponent<SandPile>();
            if (pile == null || pile.IsDug) return;

            float now = Time.time;
            int id = pile.GetInstanceID();
            if (lastHitTime.TryGetValue(id, out float prev) && now - prev < hitCooldown) return;

            lastHitTime[id] = now;
            pile.RegisterHit();
        }
    }

}