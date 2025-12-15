export default function Modal({
                                  children,
                                  onClose,
                              }: {
    children: React.ReactNode;
    onClose: () => void;
}) {
    return (
        <>
            {/* Background */}
            <div
                className="fixed inset-0 bg-black bg-opacity-40 backdrop-blur-sm z-40"
                onClick={onClose}
            ></div>

            {/* Modal */}
            <div className="fixed inset-0 flex items-center justify-center z-50 p-4">
                <div className="bg-white p-8 rounded-xl shadow-xl 
                                w-full max-w-lg max-h-[90vh] 
                                overflow-y-auto relative">

                    {/* Close button */}
                    <button
                        className="absolute top-3 right-3 text-gray-500 hover:text-red-600"
                        onClick={onClose}
                    >
                        ✕
                    </button>

                    {children}
                </div>
            </div>
        </>
    );
}

