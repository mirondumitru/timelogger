import * as React from "react";
import Projects from "./views/Projects";
import "./style.css";
import { Route, BrowserRouter as Router, Routes } from "react-router-dom";
import TimeRegistrations from "./views/TimeRegistrations";
import TimeRegistrationsAdd from "./views/TimeRegistrationsAdd";
// import TimeRegistrationsForm from "./views/TimeRegistrationsAdd";

export default function App() {
    return (
        <>
            <header className="bg-gray-900 text-white flex items-center h-12 w-full">
                <div className="container mx-auto">
                    <a className="navbar-brand" href="/">
                        Timelogger
                    </a>
                </div>
            </header>

            <main>
                <div className="container mx-auto">
                    <Router>
                        <Routes>
                            <Route path="/" element={<Projects />} />
                            <Route path="/timeRegistrations" element={<TimeRegistrations />} />
                            <Route path="/timeRegistrations/add" element={<TimeRegistrationsAdd />} />
                        </Routes>
                    </Router>
                </div>
            </main>
        </>
    );
}
