import React from 'react';
import { Table } from 'react-bootstrap';
import OrderRow from './OrderRow';

const OrderList = ({ orders, cancel, ship, deliver }) => {
    let orderRows = orders.map(o => (
        <OrderRow 
            key={o.id}
            order={o}
            cancel={() => cancel(o)}
            ship={() => ship(o)}
            deliver={() => deliver(o)} />
    ));
    
    return (
        <div>
            <Table striped hover>
                <thead>
                    <tr>
                        <th className='col-md-1'></th>
                        <th className='col-md-3'>Order Id</th>
                        <th className='col-md-2'>State</th>
                        <th className='col-md-2'>Total</th>
                        <th className='col-md-4'></th>
                    </tr>
                </thead>
                {orderRows}
            </Table>
        </div>
    );
}

export default OrderList;