import { useEffect, useState } from "react";
import Modal from "../../../components/Modal";
import Input from "../../../components/Input";
import PasswordInput from "../../../components/PasswordInput";
import { getAdmins, createAdmin, updateAdmin, deleteAdmin } from "../../../core/adminApi";

interface Admin {
    adminId: string;
    name: string;
    email: string;
    phone: string;
}

export default function AdminsTab() {
    const [admins, setAdmins] = useState<Admin[]>([]);
    const [selectedAdmin, setSelectedAdmin] = useState<Admin | null>(null);

    const [showAddModal, setShowAddModal] = useState(false);
    const [showEditModal, setShowEditModal] = useState(false);
    const [showDeleteModal, setShowDeleteModal] = useState(false);
    const [password, setPassword] = useState("");


    useEffect(() => {
        loadAdmins();
    }, []);

    async function loadAdmins() {
        const data = await getAdmins();
        setAdmins(data);
    }

    async function handleAddAdmin(e: React.FormEvent<HTMLFormElement>) {
        e.preventDefault();

        const form = e.currentTarget;
        const data = new FormData(form);

        await createAdmin({
            name: String(data.get("name") ?? ""),
            email: String(data.get("email") ?? ""),
            phone: String(data.get("phone") ?? ""),
            password,
        });

        setPassword("");
        setShowAddModal(false);
        form.reset();
        loadAdmins();
    }

    async function handleEditAdmin(e: React.FormEvent<HTMLFormElement>) {
        e.preventDefault();
        if (!selectedAdmin) return;

        const form = e.currentTarget;
        const data = new FormData(form);

        await updateAdmin(selectedAdmin.adminId, {
            name: String(data.get("name") ?? ""),
            email: String(data.get("email") ?? ""),
            phone: String(data.get("phone") ?? ""),
        });

        setShowEditModal(false);
        loadAdmins();
    }

    async function handleDeleteAdmin() {
        if (!selectedAdmin) return;

        await deleteAdmin(selectedAdmin.adminId);
        setShowDeleteModal(false);
        loadAdmins();
    }

    return (
        <div className="max-w-4xl mx-auto mt-10">
            <div className="flex justify-between items-center mb-4">
                <h2 className="text-xl font-bold text-red-600">
                    Admin Management
                </h2>

                <button
                    className="btn bg-red-600 text-white hover:bg-red-700 px-8 py-2"
                    onClick={() => setShowAddModal(true)}
                >
                    + Add Admin
                </button>
            </div>

            <div className="bg-white shadow rounded-xl p-6 text-black">
                <ul className="space-y-4">
                    {admins.map((admin) => (
                        <li
                            key={admin.adminId}
                            className="p-4 bg-[#faf6ef] rounded-xl shadow flex justify-between"
                        >
                            <div>
                                <strong>{admin.name}</strong>
                                <p>{admin.email}</p>
                                <p>{admin.phone}</p>
                            </div>

                            <div className="flex gap-3">
                                <button
                                    className="btn btn-outline border-blue-600 text-blue-600 px-5"
                                    onClick={() => {
                                        setSelectedAdmin(admin);
                                        setShowEditModal(true);
                                    }}
                                >
                                    Edit
                                </button>

                                <button
                                    className="btn btn-outline border-red-600 text-red-600 px-5"
                                    onClick={() => {
                                        setSelectedAdmin(admin);
                                        setShowDeleteModal(true);
                                    }}
                                >
                                    Delete
                                </button>
                            </div>
                        </li>
                    ))}
                </ul>
            </div>

            {showAddModal && (
                <Modal onClose={() => setShowAddModal(false)}>
                    <h3 className="text-lg font-bold mb-3 text-red-600">
                        Add New Admin
                    </h3>

                    <form onSubmit={handleAddAdmin} className="text-black space-y-4">
                        <Input  name="name" label="Full Name" required className="bg-gray-300" />
                        <Input name="email" label="Email" type="email" required className="bg-gray-300"/>
                        <Input name="phone" label="Phone" required className="bg-gray-300"/>
                        <PasswordInput
                            className="bg-gray-300"
                            value={password}
                            onChange={setPassword}
                        />


                        <button className="btn bg-red-600 text-white hover:bg-red-700 w-full py-2">
                            Save Admin
                        </button>
                    </form>
                </Modal>
            )}

            {showEditModal && selectedAdmin && (
                <Modal onClose={() => setShowEditModal(false)}>
                    <h3 className="text-lg font-bold mb-3 text-blue-600">
                        Edit Admin
                    </h3>

                    <form onSubmit={handleEditAdmin} className="space-y-4">
                        <Input name="name" defaultValue={selectedAdmin.name} label={""} />
                        <Input name="email" defaultValue={selectedAdmin.email} label={""} />
                        <Input name="phone" defaultValue={selectedAdmin.phone} label={""} />

                        <button className="btn bg-blue-600 text-white hover:bg-blue-700 w-full py-2">
                            Save Changes
                        </button>
                    </form>
                </Modal>
            )}

            {showDeleteModal && selectedAdmin && (
                <Modal onClose={() => setShowDeleteModal(false)}>
                    <h3 className="font-bold text-lg text-red-600">
                        Delete Admin
                    </h3>

                    <p className="mb-4 text-black">
                        Are you sure you want to delete{" "}
                        <strong>{selectedAdmin.name}</strong>?
                    </p>

                    <div className="flex justify-end gap-4">
                        <button
                            className="btn px-6 text-black"
                            onClick={() => setShowDeleteModal(false)}
                        >
                            Cancel
                        </button>
                        <button
                            className="btn bg-red-600 text-white hover:bg-red-700 px-6"
                            onClick={handleDeleteAdmin}
                        >
                            Delete
                        </button>
                    </div>
                </Modal>
            )}
        </div>
    );
}
