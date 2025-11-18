// src/pages/CreateDefectForm.jsx
import React from "react";
import { useForm } from "react-hook-form";
import { useNavigate } from "react-router-dom";

const CreateDefectForm = ({ onAddDefect }) => {
    const { register, handleSubmit, reset } = useForm();
    const navigate = useNavigate();

    const onSubmit = (data) => {
        navigate("/defectlist")
        onAddDefect(data); // send defect data to parent or context
        reset(); // clear the form
        
    };

    return (
        <div className="flex h-screen bg-gray-950 ">
            {/* Main Content */}
            <div className="flex-1 p-8 overflow-y-auto" >
                

                <div className="max-w-xl mx-auto p-6 bg-white rounded-lg shadow-md">
                    <h2 className="text-2xl font-bold mb-4">Log a New Defect</h2>
                    <form onSubmit={handleSubmit(onSubmit)} className="space-y-4">
                        <div>
                            <label className="block font-medium">Title</label>
                            <input
                                {...register("title", { required: true })}
                                className="w-full border p-2 rounded"
                                placeholder="Enter defect title"
                            />
                        </div>

                        <div>
                            <label className="block font-medium">Description</label>
                            <textarea
                                {...register("description", { required: true })}
                                className="w-full border p-2 rounded"
                                placeholder="Describe the defect"
                            />
                        </div>

                        <div>
                            <label className="block font-medium">Severity</label>
                            <select {...register("severity")} className="w-full border p-2 rounded">
                                <option value="Low">Low</option>
                                <option value="Medium">Medium</option>
                                <option value="High">High</option>
                                <option value="Critical">Critical</option>
                            </select>
                        </div>

                        <div>
                            <label className="block font-medium">Priority</label>
                            <select {...register("priority")} className="w-full border p-2 rounded">
                                <option value="Low">Low</option>
                                <option value="Medium">Medium</option>
                                <option value="High">High</option>
                            </select>
                        </div>

                        <button
                            type="submit"
                            className="bg-blue-600 text-white px-4 py-2 rounded hover:bg-blue-700"
                        >
                            Submit Defect
                        </button>
                    </form>
                </div>
            </div>
        </div>
            );
};

            export default CreateDefectForm;
