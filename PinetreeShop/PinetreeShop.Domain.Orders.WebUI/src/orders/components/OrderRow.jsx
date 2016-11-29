import React from 'react';
import { Button } from 'react-bootstrap';
import OrderLines from './OrderLines';

class OrderRow extends React.Component {
    constructor(props) {
        super(props);
        this.toggleOpen = this.toggleOpen.bind(this);
        this.state = { 
            isOpen: false
        }; 
    }

    toggleOpen() {
        let open = this.state.isOpen;
        this.setState({ isOpen: !open });
    }

    render() {
        let { order, cancel, prepareForShipping, ship, deliver } = this.props;

        let canPrepForShipping = order.state === 'Pending';
        let canShip = order.state === 'ReadyForShipping';
        let canDeliver = order.state === 'Shipped';
        let canCancel = order.state !== 'Delivered' && order.state !== 'Shipped' && order.state !== 'Cancelled';
        let total = order.lines.map(l => l.quantity * l.price).reduce((a,b) => a+b, 0);
        let linesRowDisplay = this.state.isOpen ? 'table-row' : 'none';

        return (
            <tbody>
                <tr>
                    <td>
                        <Button onClick={this.toggleOpen}>
                            {this.state.isOpen ? '-' : '+'}
                        </Button>
                    </td>
                    <td>{order.id}</td>
                    <td>{order.state}</td>
                    <td>{total}</td>
                    <td>
                        <Button disabled={!canPrepForShipping} onClick={prepareForShipping}>Prep for Shipping</Button>
                        <Button disabled={!canShip} onClick={ship}>Ship</Button>
                        <Button disabled={!canDeliver} onClick={deliver}>Deliver</Button>
                        <Button bsStyle='danger' disabled={!canCancel} onClick={cancel}>Cancel</Button>
                    </td>
                </tr>
                <tr style={{display: linesRowDisplay}}>
                    <td colSpan='5'>
                        <OrderLines orderLines={order.lines}/>
                    </td>
                </tr>
                <tr style={{display: 'none'}}></tr>
            </tbody>
        );
    }
}

export default OrderRow;