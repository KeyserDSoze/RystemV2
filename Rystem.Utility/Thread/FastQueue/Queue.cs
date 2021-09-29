namespace Rystem.Background
{
    public static class Queue
    {
        /// <summary>
        /// Create a queue with a T object to allow a batch operation after a maximum buffer elements or a maximum retention of time.
        /// </summary>
        /// <typeparam name="T">Object for batch</typeparam>
        /// <param name="maximumBuffer">Maximum elements per batch</param>
        /// <param name="maximumRetention">Maximum delay time for next batch</param>
        /// <param name="action">Batch operation</param>
        /// <param name="id">Queue id</param>
        /// <param name="type">Type of queue</param>
        public static void Create<T>(SequenceProperty<T> sequenceProperty, QueueType type = QueueType.FirstInFirstOut)
            => Sequences.Instance.Create<T>(sequenceProperty, type);
        /// <summary>
        /// Check if batch operation has to start. With force parameter as true you don't have to wait the maximum buffer or maximum retention to perform the batch operation.
        /// </summary>
        /// <param name="id">Queue id</param>
        /// <param name="force">You may not wait the maximum buffer or maximum retention to perform the batch operation</param>
        public static void Flush(string id = "", bool force = false)
            => Sequences.Instance.Flush(id, force);
        /// <summary>
        /// Remove the queue and emit a last forced flush.
        /// </summary>
        /// <param name="id">Queue id</param>
        public static void Destroy(string id = "")
            => Sequences.Instance.Destroy(id);
        /// <summary>
        /// Enqueue a new element.
        /// </summary>
        /// <typeparam name="T">Object for batch</typeparam>
        /// <param name="entity">Element to enqueue</param>
        /// <param name="id">Queue id</param>
        public static void Enqueue<T>(this T entity, string id = "")
            => Sequences.Instance.AddElement(entity, id);
    }
}
