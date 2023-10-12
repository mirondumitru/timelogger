import React, { useEffect, useState } from "react";
import { Column } from "react-table";
import { IProject } from "../types/IProject";
import TimeLoggerTable from "../components/Table";
import { getAll } from "../api/projectsApi";
import RedirectButton from "../components/RedirectButton";
import moment from 'moment';

function composeUrl(id: number) {
    return "timeRegistrations?projectId=" + id;
}

const columns: Array<Column> = [
    {
        Header: "Id",
        accessor: "id",
        width: 20,
        minWidth: 20,
    },
    {
        Header: "Name",
        accessor: "name"
    },
    {
        Header: "Deadline",
        accessor: "deadline",
        width: 75,
        minWidth: 75,
        Cell: props => (
            <span>{moment(props.row.values.deadline).format('YYYY-MM-DD')}</span>
        )
    },
    {
        Header: "Is Completed",
        accessor: "isCompleted",
        width: 75,
        minWidth: 75,
        Cell: props => (
            <input type="checkbox" disabled checked={props.value} />
        )
    },
    {
        Header: "Time Registrations",
        width: 65,
        minWidth: 65,
        Cell: props => (
            <RedirectButton text="Registrations" path={composeUrl(props.row.values.id)} disabled={false}></RedirectButton>
        )
    }
];

export default function Projects() {
    const [data, setData] = useState<IProject[]>([]);

    const fetchData = async () => {
        const response = await getAll({sortBy:"deadline", sortOrder:"desc"});
        setData(response);
    }

    useEffect(() => {
        fetchData();
    }, []);

    return (
        <>
            <div className="flex items-center my-6">
                <div className="w-1/2">

                </div>

               
            </div>

            <TimeLoggerTable columns={columns} data={data} />
        </>
    );
}
